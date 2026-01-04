using Scheduling.Domain.Slots;

namespace Scheduling.Application.Ports;

public interface ISlotRepository
{
    Task<Slot?> Get(Guid slotId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Slot>> GetAvailableSlots(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task Add(Slot slot, CancellationToken cancellationToken = default);

    Task Update(Slot slot, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Slot>> GetInRange(Guid doctorId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);

    Task AddBatch(IEnumerable<Slot> slots, CancellationToken cancellationToken = default);
}