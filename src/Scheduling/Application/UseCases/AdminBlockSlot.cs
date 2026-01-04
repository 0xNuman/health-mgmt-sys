using Scheduling.Application.Commands;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;

namespace Scheduling.Application.UseCases;

public sealed class AdminBlockSlot(
    ISlotRepository slots,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> Execute(BlockSlotCommand command, CancellationToken cancellationToken = default)
    {
        var slot = await slots.Get(command.SlotId, cancellationToken);

        if (slot is null)
        {
            return Result.Failure($"Slot not found with Id: {command.SlotId}");
        }

        // 2. Domain Logic: Block the slot
        slot.Block();
        await slots.Update(slot, cancellationToken);

        // 3. Conflict Resolution: "Block Always Wins"
        // Check if a booking snuck in (or existed previously) and invalidate it.
        var activeBooking = await bookings.GetActiveBySlotId(command.SlotId, cancellationToken);
        
        if (activeBooking is not null)
        {
            activeBooking.Invalidate();
            await bookings.Update(activeBooking, cancellationToken);
            
            // TODO: This is where you would publish a Domain Event
            // e.g., await events.Publish(new BookingInvalidatedEvent(activeBooking.PatientId));
            // This would trigger the Notification Service to email the user.
        }

        await unitOfWork.Commit(cancellationToken);

        return Result.Success();
    }
}