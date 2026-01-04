using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Domain.Bookings;

namespace Scheduling.Infrastructure.Repositories;

public sealed class BookingRepository(SchedulingDbContext db) : IBookingRepository
{
    public Task Add(Booking booking, CancellationToken cancellationToken = default)
    {
        db.Bookings.Add(booking);
        return Task.CompletedTask;
    }

    public async Task<Booking?> GetActiveBySlotId(Guid slotId, CancellationToken cancellationToken = default) =>
        await db.Bookings
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.SlotId == slotId && b.Status == BookingStatus.Active, cancellationToken);

    public async Task<Booking?> GetById(Guid bookingId, CancellationToken cancellationToken = default)
        => await db.Bookings.FindAsync([bookingId], cancellationToken);

    public Task Update(Booking booking, CancellationToken cancellationToken = default)
    {
        db.Bookings.Update(booking);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Booking>> GetByPatientId(Guid patientId, CancellationToken cancellationToken = default)
        => await db.Bookings
            .AsNoTracking()
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.Status)
            .ToListAsync(cancellationToken);
}