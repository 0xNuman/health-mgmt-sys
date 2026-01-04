using Scheduling.Domain.Bookings;

namespace Scheduling.Application.Ports;

public interface IBookingRepository
{
    Task Add(Booking booking, CancellationToken cancellationToken = default);

    Task<Booking?> GetActiveBySlotId(Guid slotId, CancellationToken cancellationToken = default);

    Task<Booking?> GetById(Guid bookingId, CancellationToken cancellationToken = default);

    Task Update(Booking booking, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetByPatientId(Guid patientId, CancellationToken cancellationToken = default);
}