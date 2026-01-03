using ReferenceData.Application.UseCases;
using ReferenceData.Domain.Availability;
using Web.Dtos.Admin;

namespace Web.Endpoints.Admin;

public static class AdminDoctorAvailabilityEndpoints
{
    public static IEndpointRouteBuilder MapAdminDoctorAvailabilityEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/api/admin/doctors/{doctorId:guid}/availability",
            SetAvailability);

        app.MapGet(
            "/api/admin/doctors/{doctorId:guid}/availability",
            GetAvailability);

        return app;
    }

    private static async Task<IResult> SetAvailability(
        Guid doctorId,
        DoctorAvailabilityDto dto,
        SetDoctorAvailability useCase)
    {
        var workingDays = dto.WorkingDays
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseDay)
            .ToHashSet();

        var startTime = TimeOnly.Parse(dto.DailyStartTime);
        var endTime = TimeOnly.Parse(dto.DailyEndTime);

        await useCase.Execute(
            doctorId,
            workingDays,
            startTime,
            endTime,
            dto.SlotDurationMinutes,
            dto.RollingWindowDays,
            dto.IsActive);

        return Results.Ok();
    }

    private static async Task<IResult> GetAvailability(
        Guid doctorId,
        ReferenceData.Application.Ports.IDoctorAvailabilityRepository repo)
    {
        var availability = await repo.GetByDoctorId(doctorId);
        return availability is null ? Results.NotFound() : Results.Ok(availability.ToDto());
    }

    private static DayOfWeek ParseDay(string value)
        => value switch
        {
            "Mon" => DayOfWeek.Monday,
            "Tue" => DayOfWeek.Tuesday,
            "Wed" => DayOfWeek.Wednesday,
            "Thu" => DayOfWeek.Thursday,
            "Fri" => DayOfWeek.Friday,
            "Sat" => DayOfWeek.Saturday,
            "Sun" => DayOfWeek.Sunday,
            _ => throw new ArgumentException($"Invalid day: {value}")
        };

    private static DoctorAvailabilityResponseDto ToDto(this DoctorAvailability a)
    {
        var days = string.Join(",",
            a.WorkingDays
                .OrderBy(d => d)
                .Select(ToShortName));

        return new DoctorAvailabilityResponseDto(
            days,
            a.DailyStartTime.ToString("HH:mm"),
            a.DailyEndTime.ToString("HH:mm"),
            a.SlotDurationMinutes,
            a.RollingWindowDays,
            a.IsActive);
    }

    private static string ToShortName(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "Mon",
        DayOfWeek.Tuesday => "Tue",
        DayOfWeek.Wednesday => "Wed",
        DayOfWeek.Thursday => "Thu",
        DayOfWeek.Friday => "Fri",
        DayOfWeek.Saturday => "Sat",
        DayOfWeek.Sunday => "Sun",
        _ => throw new ArgumentOutOfRangeException()
    };
}