using ReferenceData.Domain.Doctors;

namespace ReferenceData.Application.Ports;

public interface IDoctorRepository
{
    Task<Doctor?> GetById(Guid doctorId);

    Task<IReadOnlyList<Doctor>> GetAllActive();

    Task<IReadOnlyList<Doctor>> GetByIds(IEnumerable<Guid> ids);
    
    Task Add(Doctor doctor);
}