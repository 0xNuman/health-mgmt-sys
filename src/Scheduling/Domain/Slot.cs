namespace Scheduling.Domain;

public sealed class Slot
{
    public Guid Id { get; }

    public Guid DoctorId { get; }

    public DateTime StartTime { get; }

    public DateTime EndTime { get; }

    public SlotStatus Status { get; private set; }

    public Guid? AppointmentId { get; private set; }

    private Slot(Guid id, Guid doctorId, DateTime startTime, DateTime endTime, SlotStatus status)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be before end time.");
        }

        Id = id;
        DoctorId = doctorId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }

    public static Slot CreateAvailable(Guid id, Guid doctorId, DateTime startTime, DateTime endTime)
        => new Slot(id, doctorId, startTime, endTime, SlotStatus.Available);

    public void Book(Guid appointmentId)
    {
        if (Status != SlotStatus.Available)
        {
            throw new InvalidOperationException($"Cannot book slot that is not available. Status: {Status}");
        }

        Status = SlotStatus.Booked;
        AppointmentId = appointmentId;
    }

    public void CancelBooking()
    {
        if (Status != SlotStatus.Booked)
        {
            throw new InvalidOperationException($"Cannot cancel booking for slot that is not booked. Status: {Status}");
        }

        Status = SlotStatus.Available;
        AppointmentId = null;
    }

    public void Block()
    {
        if (Status == SlotStatus.Booked)
        {
            throw new InvalidOperationException("Cannot block a booked slot.");
        }

        Status = SlotStatus.Blocked;
        AppointmentId = null;
    }
}