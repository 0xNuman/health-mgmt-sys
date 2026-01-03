namespace Scheduling.Domain.Availability;

public enum AvailabilityExceptionType
{
    FullDayBlock,
    PartialDayBlock
}

public sealed class AvailabilityException
{
    public Guid DoctorId { get; private set; }

    public DateOnly Date { get; private set; }

    public AvailabilityExceptionType Type { get; private set; }

    public TimeOnly? StartTime { get; private set; }

    public TimeOnly? EndTime { get; private set; }

    public string? Reason { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private AvailabilityException() // EF
    {
    }

    public AvailabilityException(Guid doctorId, DateOnly date, AvailabilityExceptionType type, TimeOnly? startTime,
        TimeOnly? endTime, string? reason)
    {
        if (type == AvailabilityExceptionType.PartialDayBlock)
        {
            if (startTime is null || endTime is null)
            {
                throw new ArgumentException("Partial day block must have start and end time.");
            }

            if (startTime >= endTime)
            {
                throw new ArgumentException("Start time must be before end time.");
            }
        }

        if (type == AvailabilityExceptionType.FullDayBlock)
        {
            if (startTime is not null || endTime is not null)
            {
                throw new ArgumentException("Full day block must not have start or end time.");
            }
        }

        DoctorId = doctorId;
        Date = date;
        Type = type;
        StartTime = startTime;
        EndTime = endTime;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }
}