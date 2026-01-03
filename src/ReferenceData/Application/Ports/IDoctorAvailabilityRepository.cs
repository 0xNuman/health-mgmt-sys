using ReferenceData.Domain.Availability;

namespace ReferenceData.Application.Ports;

public interface IDoctorAvailabilityRepository
{
    Task<DoctorAvailability?> GetByDoctorId(Guid doctorId);

    Task Add(DoctorAvailability availability);
    
    Task Update(DoctorAvailability availability);
}