namespace Scheduling.Application.Ports;

/// <summary>
/// Defines a contract for checking the existence of a doctor in the system.
/// <remarks>
/// This is an Upstream capability interface, the doctor repository is in another module,
/// and a relevant service will be implemented there.
/// In DDD language this is a lightweight Anti-Corruption Layer (ACL).
/// </remarks>
/// </summary>
public interface IDoctorExistenceChecker
{
    Task<bool> Exists(Guid doctorId);
}