namespace Scheduling.Domain.Slots;

public sealed class Slot
{
    public Guid Id { get; private set; }
    public Guid DoctorId { get; private set; }

    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    public SlotStatus Status { get; private set; }

    private Slot()
    {
    } // EF

    public Slot(
        Guid id,
        Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("Slot end must be after start");

        Id = id;
        DoctorId = doctorId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Status = SlotStatus.Available;
    }

    public void Block()
    {
        if (Status == SlotStatus.Blocked)
            return;

        Status = SlotStatus.Blocked;
    }

    public void Unblock()
    {
        if (Status == SlotStatus.Blocked)
            Status = SlotStatus.Available;
    }

    public void MarkAsBooked()
    {
        if (Status != SlotStatus.Available)
            throw new InvalidOperationException($"Cannot book slot with status {Status}");

        Status = SlotStatus.Booked;
    }

    public void MarkAsAvailable()
    {
        // When a booking is cancelled, the slot becomes available again
        // We might want to check if it was previously Booked, but for now this is safe
        if (Status == SlotStatus.Booked)
            Status = SlotStatus.Available;
    }
}