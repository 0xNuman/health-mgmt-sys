using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Commands;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;
using Scheduling.Application.UseCases;
using Scheduling.Domain;
using Scheduling.Infrastructure;

namespace Scheduling.Tests.Infrastructure;

public class SlotConcurrencyTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<SchedulingDbContext> _dbOptions;

    public SlotConcurrencyTests()
    {
        // 🔑 Shared in-memory database for all tests
        _connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Only_one_booking_succeeds_under_concurrent_requests()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var slotId = Guid.NewGuid();

        SeedAvailableSlot(slotId, doctorId);

        // Two separate use cases simulating concurrent requests
        var task1 = Task.Run(() => TryBook(slotId));
        var task2 = Task.Run(() => TryBook(slotId));

        // Act
        var results = await Task.WhenAll(task1, task2);

        // Assert
        Assert.Single(results, r => r.Success);
        Assert.Single(results, r => !r.Success);

        await using var verificationContext = CreateDbContext();
        var slot = verificationContext.Slots.Single(s => s.Id == slotId);

        Assert.Equal((int)SlotStatus.Booked, slot.Status);
        Assert.NotNull(slot.AppointmentId);
    }

    private async Task<BookingResult> TryBook(Guid slotId)
    {
        await using var context = CreateDbContext();
        ISlotRepository repo = new SlotRepository(context);
        var useCase = new BookSlot(repo);

        return await useCase.Execute(
            new BookSlotCommand(
                slotId,
                Guid.NewGuid()));
    }

    private void SeedAvailableSlot(Guid slotId, Guid doctorId)
    {
        using var context = CreateDbContext();

        context.Slots.Add(new SlotEntity
        {
            Id = slotId,
            DoctorId = doctorId,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(15),
            Status = (int)SlotStatus.Available,
            AppointmentId = null
        });

        context.SaveChanges();
    }

    private SchedulingDbContext CreateDbContext() => new(_dbOptions);

    public void Dispose()
    {
        _connection.Dispose();
    }
}