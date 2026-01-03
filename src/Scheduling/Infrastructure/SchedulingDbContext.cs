using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Availability;

namespace Scheduling.Infrastructure;

public sealed class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    public DbSet<SlotEntity> Slots => Set<SlotEntity>();

    public DbSet<AvailabilityException> AvailabilityExceptions => Set<AvailabilityException>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SlotEntity>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<SlotEntity>()
            .HasIndex(s => new { s.DoctorId, s.StartTime })
            .IsUnique();

        modelBuilder.Entity<AvailabilityException>(entity =>
        {
            entity.HasKey(x => new { x.DoctorId, x.Date });

            entity.Property(x => x.Type)
                .HasConversion<string>();

            entity.Property(x => x.StartTime)
                .HasConversion<TimeSpan?>(
                    t => t.HasValue ? t.Value.ToTimeSpan() : null,
                    t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);

            entity.Property(x => x.EndTime)
                .HasConversion<TimeSpan?>(
                    t => t.HasValue ? t.Value.ToTimeSpan() : null,
                    t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        });
    }
}