using Microsoft.EntityFrameworkCore;
using ReferenceData.Application.Ports;
using ReferenceData.Domain.Availability;

namespace ReferenceData.Infrastructure.Repositories;

public sealed class DoctorAvailabilityRepository(ReferenceDataDbContext db) : IDoctorAvailabilityRepository
{
    public async Task<DoctorAvailability?> GetByDoctorId(Guid doctorId)
        => await db.DoctorAvailabilities.SingleOrDefaultAsync(a => a.DoctorId == doctorId);

    public async Task Add(DoctorAvailability availability)
    {
        db.DoctorAvailabilities.Add(availability);
        await db.SaveChangesAsync();
    }

    public async Task Update(DoctorAvailability availability)
    {
        db.DoctorAvailabilities.Update(availability);
        await db.SaveChangesAsync();
    }
}