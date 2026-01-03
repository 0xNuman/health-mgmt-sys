using ReferenceData.Application.Ports;
using ReferenceData.Domain.Availability;

namespace ReferenceData.Application.UseCases;

public sealed class SetDoctorAvailability(IDoctorRepository doctors, IDoctorAvailabilityRepository availability)
{
    public async Task Execute(
        Guid doctorId,
        IReadOnlySet<DayOfWeek> workingDays,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDurationMinutes,
        int rollingWindowDays,
        bool isActive)
    {
        var doctor = doctors.GetById(doctorId);
        if (doctor is null)
            throw new InvalidOperationException("Doctor does not exist");

        var existing = await availability.GetByDoctorId(doctorId);

        if (existing is null)
        {
            var availability1 = new DoctorAvailability(
                doctorId,
                workingDays,
                startTime,
                endTime,
                slotDurationMinutes,
                rollingWindowDays,
                isActive);

            await availability.Add(availability1);
            return;
        }

        existing.Replace(
            workingDays,
            startTime,
            endTime,
            slotDurationMinutes,
            rollingWindowDays,
            isActive);

        await availability.Update(existing);
    }
}