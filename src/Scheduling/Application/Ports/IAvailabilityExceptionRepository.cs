using Scheduling.Domain.Availability;

namespace Scheduling.Application.Ports;

public interface IAvailabilityExceptionRepository
{
    Task<AvailabilityException?> Get(Guid doctorId, DateOnly date);

    Task<IReadOnlyList<AvailabilityException>> GetInRange(
        Guid doctorId,
        DateOnly from,
        DateOnly to
    );

    Task Add(AvailabilityException exception);

    Task Delete(Guid doctorId, DateOnly date);
}