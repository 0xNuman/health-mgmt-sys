using Scheduling.Application.Ports;

namespace Scheduling.Application.UseCases;

public sealed class DeleteAvailabilityException(
    IAvailabilityExceptionRepository exceptions,
    IUnitOfWork unitOfWork)
{
    public async Task Execute(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default)
    {
        await exceptions.Delete(doctorId, date, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
    }
}