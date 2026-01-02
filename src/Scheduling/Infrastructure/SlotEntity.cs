namespace Scheduling.Infrastructure;

public sealed class SlotEntity
{
    public Guid Id { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Status { get; set; }

    public Guid? AppointmentId { get; set; }
}