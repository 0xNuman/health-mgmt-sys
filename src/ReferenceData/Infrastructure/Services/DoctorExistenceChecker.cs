using ReferenceData.Application.Ports;
using Scheduling.Application.Ports;

namespace ReferenceData.Infrastructure.Services;

public sealed class DoctorExistenceChecker(IDoctorRepository doctors) : IDoctorExistenceChecker
{
    public async Task<bool> Exists(Guid doctorId, CancellationToken cancellationToken = default)
        => await doctors.GetById(doctorId) is not null;
}