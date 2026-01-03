namespace ReferenceData.Domain.Availability;

public sealed class DoctorAvailability
{
    public Guid DoctorId { get; private set; }

    public IReadOnlySet<DayOfWeek> WorkingDays { get; private set; } = new HashSet<DayOfWeek>();

    public TimeOnly DailyStartTime { get; private set; }

    public TimeOnly DailyEndTime { get; private set; }

    public int SlotDurationMinutes { get; private set; }

    public int RollingWindowDays { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private DoctorAvailability()
    {
    }

    public DoctorAvailability(
        Guid doctorId,
        IReadOnlySet<DayOfWeek> workingWorkingDays,
        TimeOnly dailyStartTime,
        TimeOnly dailyEndTime,
        int slotDurationMinutes,
        int rollingWindowDays,
        bool isActive
    )
    {
        if (workingWorkingDays is null || workingWorkingDays.Count == 0)
        {
            throw new ArgumentException("At least one day must be specified", nameof(workingWorkingDays));
        }

        if (dailyStartTime >= dailyEndTime)
        {
            throw new ArgumentException("Start time must be before end time.", nameof(dailyStartTime));
        }

        if (slotDurationMinutes <= 0)
        {
            throw new ArgumentException("Slot duration must be greater than zero.", nameof(slotDurationMinutes));
        }

        if (rollingWindowDays < 0)
        {
            throw new ArgumentException("Rolling window must be zero or greater.", nameof(rollingWindowDays));
        }

        DoctorId = doctorId;
        WorkingDays = workingWorkingDays;
        DailyStartTime = dailyStartTime;
        DailyEndTime = dailyEndTime;
        SlotDurationMinutes = slotDurationMinutes;
        RollingWindowDays = rollingWindowDays;
        IsActive = isActive;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Replace(
        IReadOnlySet<DayOfWeek> workingDays,
        TimeOnly dailyStartTime,
        TimeOnly dailyEndTime,
        int slotDurationMinutes,
        int rollingWindowDays,
        bool isActive
    )
    {
        if (workingDays is null || workingDays.Count == 0)
        {
            throw new ArgumentException("At least one day must be specified", nameof(workingDays));
        }

        if (dailyStartTime >= dailyEndTime)
        {
            throw new ArgumentException("Start time must be before end time.", nameof(dailyStartTime));
        }

        if (slotDurationMinutes <= 0)
        {
            throw new ArgumentException("Slot duration must be greater than zero.", nameof(slotDurationMinutes));
        }

        if (rollingWindowDays < 0)
        {
            throw new ArgumentException("Rolling window must be zero or greater.", nameof(rollingWindowDays));
        }

        WorkingDays = workingDays;
        DailyStartTime = dailyStartTime;
        DailyEndTime = dailyEndTime;
        SlotDurationMinutes = slotDurationMinutes;
        RollingWindowDays = rollingWindowDays;
        IsActive = isActive;

        UpdatedAt = DateTime.UtcNow;
    }
}