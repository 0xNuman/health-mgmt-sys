using ReferenceData.Application.Ports;
using Scheduling.Application.UseCases;

namespace Web.Services;

public record CompositeBookingDto(
    Guid Id,
    Guid SlotId,
    string DoctorName,
    string Date,
    string StartTime,
    string EndTime,
    string Status
);

public class PatientBookingsOrchestrator(
    GetPatientBookings getBookings,
    IDoctorRepository doctorRepo)
{
    public async Task<IReadOnlyList<CompositeBookingDto>> GetCompositeBookings(Guid patientId)
    {
        // 1. Get raw bookings from Scheduling module
        var bookings = await getBookings.Execute(patientId);
        
        if (!bookings.Any())
        {
            return Array.Empty<CompositeBookingDto>();
        }

        // 2. Get unique Doctor IDs
        var doctorIds = bookings.Select(b => b.DoctorId).Distinct();

        // 3. Batch fetch Doctor details from ReferenceData module
        var doctors = await doctorRepo.GetByIds(doctorIds);
        var doctorMap = doctors.ToDictionary(d => d.Id, d => d.FullName);

        // 4. Compose final result
        return bookings.Select(b => new CompositeBookingDto(
            b.BookingId,
            b.SlotId,
            doctorMap.GetValueOrDefault(b.DoctorId, "Unknown Doctor"),
            b.Date,
            b.StartTime,
            b.EndTime,
            b.Status
        )).ToList();
    }
}
