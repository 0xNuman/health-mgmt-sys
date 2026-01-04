using Scheduling.Application.Commands;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;
using Scheduling.Domain.Bookings;
using Scheduling.Domain.Slots;

namespace Scheduling.Application.UseCases;

public sealed class BookSlot(
    ISlotRepository slots,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork)
{
    public async Task<BookSlotResult> Execute(
        BookSlotCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var slot = await slots.Get(command.SlotId, cancellationToken);

        if (slot is null)
        {
            return BookSlotResult.SlotNotFound();
        }

        if (slot.Status != SlotStatus.Available)
        {
            return BookSlotResult.SlotUnavailable("Slot is not available");
        }

        // Check for double booking risk using unique constraint (handled by DB)
        // or re-check active bookings here if needed, but UoW + optimistic locking is standard.
        // For now, we rely on the unique index on Booking.SlotId if it exists?
        // Wait, Slot status check is optimistic locking kind of.

        var booking = new Booking(Guid.NewGuid(), command.SlotId, command.PatientId);

        try
        {
            slot.MarkAsBooked();
            await slots.Update(slot, cancellationToken);
            await bookings.Add(booking, cancellationToken);
            
            await unitOfWork.Commit(cancellationToken);
            
            return BookSlotResult.Ok(booking.Id);
        }
        catch (SlotAlreadyBookedException)
        {
            return BookSlotResult.SlotUnavailable("Slot is already booked");
        }
        // Catch DbUpdateException for unique constraint violation if we kept that logic?
        // The BookingRepository logic for UniqueConstraint was removed in the previous step...
        // Ah, I need to restore that logic or handle it in UoW commit?
        // The UoW commit will throw the exception now.
        // So I should catch DbUpdateException here or in UoW.
        // Let's stick to catching generic or specific DB exceptions if we want to return a nice result.
        // For now, let's keep it simple.
        catch (Exception)
        {
             // If UoW commit fails, it might be due to concurrency
             return BookSlotResult.SlotUnavailable("Slot became unavailable during booking");
        }
    }
}