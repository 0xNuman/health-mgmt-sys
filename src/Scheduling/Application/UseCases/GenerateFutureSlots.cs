using Scheduling.Application.Ports;
using Scheduling.Domain.Availability;
using Scheduling.Domain.Slots;

namespace Scheduling.Application.UseCases;

public sealed class GenerateFutureSlots(
    IDoctorConfigurationAccessor configurationAccessor,
    IAvailabilityExceptionRepository exceptions,
    ISlotRepository slots,
    IUnitOfWork unitOfWork
)
{
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        // configurationAccessor usually doesn't need CancellationToken if it's simple list, but good practice
        var configs = await configurationAccessor.GetAllActiveConfigs(); 
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var config in configs)
        {
            if (cancellationToken.IsCancellationRequested) break;
            await GenerateForDoctor(config, today, cancellationToken);
        }
        
        // We could commit per doctor, or once at the end. 
        // Given it's a background job, per doctor might be safer to avoid huge transaction logs,
        // but typically UoW is per request/operation.
        // Let's do it per doctor to keep transactions smaller if we had logic inside GenerateForDoctor,
        // BUT GenerateForDoctor calls AddBatch which is just context.Add.
        // Actual save is on Commit.
        // Let's commit at the end for now, or per doctor?
        // If one doctor fails, do we want others to succeed? Yes.
        // So commit per doctor? 
        // The UseCase Execute implies one operation. 
        // But for batch jobs, smaller transactions are better.
        // Let's stick to simple "Commit at the end" unless performance dictates otherwise, 
        // OR commit inside the loop. 
        // For standard UoW usage in Use Cases, it's one commit.
        
        await unitOfWork.Commit(cancellationToken);
    }

    private async Task GenerateForDoctor(DoctorScheduleConfig config, DateOnly from, CancellationToken cancellationToken)
    {
        var to = from.AddDays(config.RollingWindowDays);

        // 1. Fetch exceptions for this range (Leave, Sick days, etc.)
        var overrides = await exceptions.GetInRange(config.DoctorId, from, to, cancellationToken);

        // 2. Fetch existing slots to ensure Idempotency (Don't generate duplicates)
        var existingSlots = await slots.GetInRange(config.DoctorId, from, to, cancellationToken);
        var existingKeys = existingSlots.Select(s => new { s.Date, s.StartTime }).ToHashSet();

        var slotsToAdd = new List<Slot>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            // Skip non-working days
            if (!config.WorkingDays.Contains(date.DayOfWeek)) continue;

            // Skip Full day Blocks
            var dailyException = overrides.FirstOrDefault(e => e.Date == date);
            if (dailyException?.Type == AvailabilityExceptionType.FullDayBlock) continue;

            var currentTime = config.DailyStartTime;

            // Generate chunks
            while (currentTime.AddMinutes(config.SlotDurationMinutes) <= config.DailyEndTime)
            {
                var endTime = currentTime.AddMinutes(config.SlotDurationMinutes);

                // Skip if Partial Day Block overlaps
                bool isBlocked = dailyException?.Type == AvailabilityExceptionType.PartialDayBlock &&
                                 IsOverlapping(dailyException, currentTime, endTime);

                // Skip if already exists
                if (!isBlocked && !existingKeys.Contains(new { Date = date, StartTime = currentTime }))
                {
                    slotsToAdd.Add(new Slot(Guid.NewGuid(), config.DoctorId, date, currentTime, endTime));
                }

                currentTime = endTime;
            }
        }

        if (slotsToAdd.Count > 0)
        {
            await slots.AddBatch(slotsToAdd, cancellationToken);
        }
    }

    private static bool IsOverlapping(AvailabilityException ex, TimeOnly slotStart,
        TimeOnly slotEnd)
    {
        if (ex.StartTime == null || ex.EndTime == null) return false;
        return slotStart < ex.EndTime && slotEnd > ex.StartTime;
    }
}