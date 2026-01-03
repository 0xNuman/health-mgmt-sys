using ReferenceData.Application.Ports;
using Scheduling.Application.Ports;

namespace ReferenceData.Infrastructure.Services;

public sealed class DoctorExistenceChecker(IDoctorRepository doctors) : IDoctorExistenceChecker
{
    public async Task<bool> Exists(Guid doctorId)
        => await doctors.GetById(doctorId) is not null;
}