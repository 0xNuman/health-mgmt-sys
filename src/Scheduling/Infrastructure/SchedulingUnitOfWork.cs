using Scheduling.Application.Ports;

namespace Scheduling.Infrastructure;

public sealed class SchedulingUnitOfWork(SchedulingDbContext db) : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }
}
