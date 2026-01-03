using Microsoft.EntityFrameworkCore;
using ReferenceData.Domain.Availability;
using ReferenceData.Domain.Doctors;
using ReferenceData.Domain.Patients;

namespace ReferenceData.Infrastructure;

public sealed class ReferenceDataDbContext(DbContextOptions<ReferenceDataDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Patient>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<DoctorAvailability>(entity =>
        {
            entity.HasKey(a => a.DoctorId);

            entity.Property(a => a.WorkingDays)
                .HasConversion<string>(
                    c => string.Join(",", c),
                    c => c.Split(',').Select(Enum.Parse<DayOfWeek>).ToHashSet());

            entity.Property(a => a.DailyStartTime)
                .HasConversion(
                    t => t.ToTimeSpan(),
                    t => TimeOnly.FromTimeSpan(t));

            entity.Property(a => a.DailyEndTime)
                .HasConversion(
                    t => t.ToTimeSpan(),
                    t => TimeOnly.FromTimeSpan(t));
        });
    }
}