using Scheduling.Application.Commands;
using Scheduling.Application.UseCases;
using Scheduling.Domain;

namespace Scheduling.Tests.Application.UseCases;

public class BookSlotTests
{
    [Fact]
    public async Task Booking_succeeds_for_available_slot()
    {
        var slot = Slot.CreateAvailable(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now.AddMinutes(15));

        var repo = new FakeSlotRepository(slot);
        var useCase = new BookSlot(repo);

        var result = await useCase.Execute(new BookSlotCommand(slot.Id, Guid.NewGuid()));

        Assert.True(result.Success);
        Assert.Equal(SlotStatus.Booked, slot.Status);
    }

    [Fact]
    public async Task Booking_fails_if_slot_not_found()
    {
        var repo = new FakeSlotRepository(null);
        var useCase = new BookSlot(repo);
        var slotId = Guid.NewGuid();

        var result = await useCase.Execute(new BookSlotCommand(slotId, Guid.NewGuid()));

        Assert.False(result.Success);
        Assert.Equal($"Slot not found with Id: '{slotId:D}'", result.Error);
    }

    [Fact]
    public async Task Booking_fails_if_slot_already_booked()
    {
        var slot = Slot.CreateAvailable(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(15));

        slot.Book(Guid.NewGuid());

        var repo = new FakeSlotRepository(slot);
        var useCase = new BookSlot(repo);

        var result = await useCase.Execute(
            new BookSlotCommand(slot.Id, Guid.NewGuid()));

        Assert.False(result.Success);
    }
}