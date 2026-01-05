using Scheduling.Application.Ports;

namespace Scheduling.Application.UseCases;

public sealed record PatientBookingDto(
    Guid BookingId,
    Guid SlotId,
    Guid DoctorId,
    string Date,
    string StartTime,
    string EndTime,
    string Status
);

public sealed class GetPatientBookings(IBookingRepository bookings, ISlotRepository slots)
{
    public async Task<IReadOnlyList<PatientBookingDto>> Execute(Guid patientId)
    {
        var myBookings = await bookings.GetByPatientId(patientId);
        var results = new List<PatientBookingDto>();

        // Note: For MVP, looping is fine. For Prod, use a JOIN query in Repo.
        foreach (var b in myBookings)
        {
            var slot = await slots.Get(b.SlotId);
            if (slot is not null)
            {
                results.Add(new PatientBookingDto(
                    b.Id,
                    b.SlotId,
                    slot.DoctorId,
                    slot.Date.ToString("yyyy-MM-dd"),
                    slot.StartTime.ToString("HH:mm"),
                    slot.EndTime.ToString("HH:mm"),
                    b.Status.ToString()
                ));
            }
        }

        return results;
    }
}