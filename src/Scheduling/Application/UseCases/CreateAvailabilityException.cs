using Scheduling.Application.Ports;
using Scheduling.Domain.Availability;

namespace Scheduling.Application.UseCases;

public class CreateAvailabilityException(
    IDoctorExistenceChecker doctorExistenceChecker,
    IAvailabilityExceptionRepository exceptions)
{
    public async Task Execute(
        Guid doctorId,
        DateOnly date,
        AvailabilityExceptionType type,
        TimeOnly? startTime,
        TimeOnly? endTime,
        string? reason
    )
    {
        if (!await doctorExistenceChecker.Exists(doctorId))
        {
            throw new InvalidOperationException("Doctor does not exist.");
        }

        var existing = await exceptions.Get(doctorId, date);
        if (existing is not null)
        {
            throw new InvalidOperationException("An availability exception already exists for this doctor and date");
        }

        await exceptions.Add(new AvailabilityException(doctorId, date, type, startTime, endTime, reason));
    }
}