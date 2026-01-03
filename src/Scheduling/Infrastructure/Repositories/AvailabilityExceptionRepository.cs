using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Ports;
using Scheduling.Domain.Availability;

namespace Scheduling.Infrastructure.Repositories;

public sealed class AvailabilityExceptionRepository(SchedulingDbContext db) : IAvailabilityExceptionRepository
{
    public async Task<AvailabilityException?> Get(Guid doctorId, DateOnly date) =>
        await db.AvailabilityExceptions.SingleOrDefaultAsync(a => a.DoctorId == doctorId && a.Date == date);

    public async Task<IReadOnlyList<AvailabilityException>> GetInRange(Guid doctorId, DateOnly from, DateOnly to)
        => await db.AvailabilityExceptions.Where(a =>
                a.DoctorId == doctorId &&
                a.Date >= from &&
                a.Date <= to)
            .ToListAsync();

    public async Task Add(AvailabilityException exception)
    {
        db.AvailabilityExceptions.Add(exception);
        await db.SaveChangesAsync();
    }

    public async Task Delete(Guid doctorId, DateOnly date)
    {
        var entity = await Get(doctorId, date);
        if (entity is null)
        {
            return;
        }

        db.AvailabilityExceptions.Remove(entity);
        await db.SaveChangesAsync();
    }
}