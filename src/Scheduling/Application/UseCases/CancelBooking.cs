using Scheduling.Application.Commands;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;
using Scheduling.Domain.Slots;

namespace Scheduling.Application.UseCases;

public sealed class CancelBooking(IBookingRepository bookings, ISlotRepository slots)
{
    public async Task<Result> Execute(CancelBookingCommand command)
    {
        var booking = await bookings.GetById(command.BookingId);

        if (booking is null)
        {
            return Result.Failure("Booking not found");
        }

        try
        {
            booking.Cancel();
            
            var slot = await slots.Get(booking.SlotId);
            if (slot != null)
            {
                slot.MarkAsAvailable();
                await slots.Update(slot);
            }
        }
        catch (InvalidOperationException e)
        {
            return Result.Failure(e.Message);
        }

        await bookings.Update(booking);

        return Result.Success();
    }
}