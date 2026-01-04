namespace Scheduling.Application.Ports;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken = default);
}
