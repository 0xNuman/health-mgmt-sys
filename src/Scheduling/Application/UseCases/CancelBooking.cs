using Scheduling.Application.Commands;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;
using Scheduling.Domain.Slots;

namespace Scheduling.Application.UseCases;

public sealed class CancelBooking(
    IBookingRepository bookings,
    ISlotRepository slots,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> Execute(CancelBookingCommand command, CancellationToken cancellationToken = default)
    {
        var booking = await bookings.GetById(command.BookingId, cancellationToken);

        if (booking is null)
        {
            return Result.Failure("Booking not found");
        }

        try
        {
            booking.Cancel();
            
            var slot = await slots.Get(booking.SlotId, cancellationToken);
            if (slot != null)
            {
                slot.MarkAsAvailable();
                await slots.Update(slot, cancellationToken);
            }
        }
        catch (InvalidOperationException e)
        {
            return Result.Failure(e.Message);
        }

        await bookings.Update(booking, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return Result.Success();
    }
}