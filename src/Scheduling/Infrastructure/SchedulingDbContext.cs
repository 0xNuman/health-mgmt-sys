using Microsoft.EntityFrameworkCore;

namespace Scheduling.Infrastructure;

public sealed class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    public DbSet<SlotEntity> Slots => Set<SlotEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SlotEntity>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<SlotEntity>()
            .HasIndex(s => new { s.DoctorId, s.StartTime })
            .IsUnique();
    }
}