using Scheduling.Application.Commands;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;

namespace Scheduling.Application.UseCases;

public sealed class BookSlot
{
    private readonly ISlotRepository _slots;

    public BookSlot(ISlotRepository slots)
    {
        ArgumentNullException.ThrowIfNull(slots);

        _slots = slots;
    }

    public async Task<BookingResult> Execute(BookSlotCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var slot = await _slots.GetById(command.SlotId);

        if (slot is null)
        {
            return BookingResult.Fail($"Slot not found with Id: '{command.SlotId:D}'");
        }

        try
        {
            slot.Book(command.AppointmentId);
            await _slots.Save(slot);

            return BookingResult.Ok();
        }
        catch (SlotAlreadyBookedException e)
        {
            return BookingResult.Fail(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BookingResult.Fail(e.Message);
        }
    }
}