using Scheduling.Domain.Availability;

namespace Scheduling.Application.Ports;

public interface IAvailabilityExceptionRepository
{
    Task<AvailabilityException?> Get(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AvailabilityException>> GetInRange(
        Guid doctorId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default
    );

    Task Add(AvailabilityException exception, CancellationToken cancellationToken = default);

    Task Delete(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default);
}