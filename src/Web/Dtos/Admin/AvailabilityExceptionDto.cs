namespace Web.Dtos.Admin;

public sealed record AvailabilityExceptionDto(
    string Date,
    string Type,
    string? StartTime,
    string? EndTime,
    string? Reason
);