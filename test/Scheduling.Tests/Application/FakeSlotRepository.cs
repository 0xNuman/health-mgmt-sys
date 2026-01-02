using Scheduling.Application.Ports;
using Scheduling.Domain;

namespace Scheduling.Tests.Application;

public class FakeSlotRepository(Slot? slot) : ISlotRepository
{
    public Task<Slot?> GetById(Guid slotId) => Task.FromResult(slot?.Id == slotId ? slot : null);

    public Task Save(Slot slotToSave)
    {
        // No-op for tests
        return Task.CompletedTask;
    }
}