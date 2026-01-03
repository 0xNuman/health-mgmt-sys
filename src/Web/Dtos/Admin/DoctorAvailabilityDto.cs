namespace Web.Dtos.Admin;

public sealed record DoctorAvailabilityDto(
    string WorkingDays,
    string DailyStartTime,
    string DailyEndTime,
    int SlotDurationMinutes,
    int RollingWindowDays,
    bool IsActive
);