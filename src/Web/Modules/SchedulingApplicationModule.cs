using Scheduling.Application.UseCases;

namespace Web.Modules;

public static class SchedulingApplicationModule
{
    public static IServiceCollection AddSchedulingApplication(
        this IServiceCollection services)
    {
        services.AddScoped<BookSlot>();
        services.AddScoped<CancelBooking>();
        services.AddScoped<AdminBlockSlot>();
        
        services.AddScoped<CreateAvailabilityException>();
        services.AddScoped<DeleteAvailabilityException>();

        return services;
    }
}