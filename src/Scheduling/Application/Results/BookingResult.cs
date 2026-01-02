namespace Scheduling.Application.Results;

public sealed class BookingResult
{
    public bool Success { get; }

    public string? Error { get; }

    private BookingResult(bool success, string? error)
    {
        Success = success;
        Error = error;
    }

    public static BookingResult Ok() => new BookingResult(true, null);

    public static BookingResult Fail(string error) => new BookingResult(false, error);
}