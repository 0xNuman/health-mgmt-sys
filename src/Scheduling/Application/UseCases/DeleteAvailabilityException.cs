using Scheduling.Application.Ports;

namespace Scheduling.Application.UseCases;

public sealed class DeleteAvailabilityException(IAvailabilityExceptionRepository exceptions)
{
    public async Task Execute(Guid doctorId, DateOnly date)
        => await exceptions.Delete(doctorId, date);
}