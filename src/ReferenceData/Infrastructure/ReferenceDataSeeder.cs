using ReferenceData.Domain.Availability;
using ReferenceData.Domain.Doctors;
using ReferenceData.Domain.Patients;

namespace ReferenceData.Infrastructure;

public sealed class ReferenceDataSeeder(ReferenceDataDbContext db)
{
    public async Task SeedAsync()
    {
        await db.Database.EnsureCreatedAsync();

        if (db.Doctors.Any())
        {
            return;
        }

        var doctors = new[]
        {
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Dr. Thor",
                Specialty = "Neuro Surgeon",
                IsActive = true
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Dr. Steve Rogers",
                Specialty = "Orthopaedic Surgeon",
                IsActive = true
            }
        };

        var patients = new[]
        {
            new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "Bruce Banner",
                Phone = "9000000000",
                Email = "hulk@marvel.com"
            },
            new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "Tony Stark",
                Phone = "8000000000",
                Email = "tony@marvel.com"
            }
        };

        var doctorAvailabilities = new[]
        {
            new DoctorAvailability(doctors[0].Id,
                new HashSet<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
                new TimeOnly(10, 0),
                new TimeOnly(11, 0),
                10,
                14,
                true),
            new DoctorAvailability(doctors[1].Id,
                new HashSet<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
                new TimeOnly(10, 0),
                new TimeOnly(11, 0),
                10,
                7,
                true)
        };

        await db.Doctors.AddRangeAsync(doctors);
        await db.Patients.AddRangeAsync(patients);
        await db.DoctorAvailabilities.AddRangeAsync(doctorAvailabilities);
        await db.SaveChangesAsync();
    }
}