using Scheduling.Application.UseCases;
using Scheduling.Domain.Availability;
using Web.Dtos.Admin;

namespace Web.Endpoints.Admin;

public static class AdminAvailabilityExceptionEndpoints
{
    public static IEndpointRouteBuilder MapAdminAvailabilityExceptionEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/doctors/{doctorId:guid}/availability-exceptions", Create);
        app.MapGet("/api/admin/doctors/{doctorId:guid}/availability-exceptions", GetRange);
        app.MapDelete("/api/admin/doctors/{doctorId:guid}/availability-exceptions/{date}", Delete);

        return app;
    }

    private static async Task<IResult> Create(
        Guid doctorId,
        AvailabilityExceptionDto dto,
        CreateAvailabilityException useCase)
    {
        var date = DateOnly.Parse(dto.Date);
        var type = Enum.Parse<AvailabilityExceptionType>(dto.Type);

        TimeOnly? start = dto.StartTime is null
            ? null
            : TimeOnly.Parse(dto.StartTime);

        TimeOnly? end = dto.EndTime is null
            ? null
            : TimeOnly.Parse(dto.EndTime);

        await useCase.Execute(
            doctorId,
            date,
            type,
            start,
            end,
            dto.Reason);

        return Results.Ok();
    }

    private static async Task<IResult> GetRange(
        Guid doctorId,
        DateOnly from,
        DateOnly to,
        Scheduling.Application.Ports.IAvailabilityExceptionRepository repo)
    {
        var results = await repo.GetInRange(doctorId, from, to);

        var response = results.Select(e => new
        {
            Date = e.Date.ToString("yyyy-MM-dd"),
            Type = e.Type.ToString(),
            StartTime = e.StartTime?.ToString("HH:mm"),
            EndTime = e.EndTime?.ToString("HH:mm"),
            e.Reason
        });

        return Results.Ok(response);
    }

    private static async Task<IResult> Delete(
        Guid doctorId,
        DateOnly date,
        DeleteAvailabilityException useCase)
    {
        await useCase.Execute(doctorId, date);
        return Results.NoContent();
    }
}