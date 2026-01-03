namespace Web.Dtos.Admin;

public sealed record DoctorAvailabilityResponseDto(
    string WorkingDays,
    string DailyStartTime,
    string DailyEndTime,
    int SlotDurationMinutes,
    int RollingWindowDays,
    bool IsActive
);