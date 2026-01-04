using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Ports;
using Scheduling.Domain.Availability;

namespace Scheduling.Infrastructure.Repositories;

public sealed class AvailabilityExceptionRepository(SchedulingDbContext db) : IAvailabilityExceptionRepository
{
    public async Task<AvailabilityException?> Get(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default) =>
        await db.AvailabilityExceptions.SingleOrDefaultAsync(a => a.DoctorId == doctorId && a.Date == date, cancellationToken);

    public async Task<IReadOnlyList<AvailabilityException>> GetInRange(Guid doctorId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        => await db.AvailabilityExceptions.Where(a =>
                a.DoctorId == doctorId &&
                a.Date >= from &&
                a.Date <= to)
            .ToListAsync(cancellationToken);

    public Task Add(AvailabilityException exception, CancellationToken cancellationToken = default)
    {
        db.AvailabilityExceptions.Add(exception);
        return Task.CompletedTask;
    }

    public async Task Delete(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var entity = await Get(doctorId, date, cancellationToken);
        if (entity is null)
        {
            return;
        }

        db.AvailabilityExceptions.Remove(entity);
    }
}