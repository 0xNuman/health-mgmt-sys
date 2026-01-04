using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Ports;
using Scheduling.Domain.Slots;

namespace Scheduling.Infrastructure.Repositories;

public sealed class SlotRepository(SchedulingDbContext db) : ISlotRepository
{
    public async Task<Slot?> Get(Guid slotId, CancellationToken cancellationToken = default)
        => await db.Slots.SingleOrDefaultAsync(s => s.Id == slotId, cancellationToken);

    public async Task<IReadOnlyList<Slot>> GetAvailableSlots(Guid doctorId, DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await db.Slots
            .AsNoTracking()
            .Where(s =>
                s.DoctorId == doctorId &&
                s.Date == date &&
                s.Status == SlotStatus.Available)
            .ToListAsync(cancellationToken);
    }

    public Task Add(Slot slot, CancellationToken cancellationToken = default)
    {
        db.Slots.Add(slot);
        return Task.CompletedTask;
    }

    public Task Update(Slot slot, CancellationToken cancellationToken = default)
    {
        db.Slots.Update(slot);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Slot>> GetInRange(Guid doctorId, DateOnly from, DateOnly to,
        CancellationToken cancellationToken = default)
        => await db.Slots
            .AsNoTracking()
            .Where(s =>
                s.DoctorId == doctorId &&
                s.Date >= from &&
                s.Date <= to)
            .ToListAsync(cancellationToken);

    public async Task AddBatch(IEnumerable<Slot> slots, CancellationToken cancellationToken = default)
    {
        await db.Slots.AddRangeAsync(slots, cancellationToken);
    }
}