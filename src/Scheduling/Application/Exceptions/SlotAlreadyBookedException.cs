namespace Scheduling.Application.Exceptions;

public sealed class SlotAlreadyBookedException() : Exception("Slot is already booked");