using HotelManager.Shared.Query;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Factories;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManager.Shared.Extensions;

public static class Registrations
{
    public static IServiceCollection RegisterSharedServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AvailabilityQuery>());
        // I'm using singletons here because I want to keep the data in memory for the entire application lifetime which will be probably short.
        // In a real-world application, I would use different type based on service beahviour and API type.
        services.AddSingleton<IInMemoryRepository<Booking>, InMemoryBookingsRepository>();
        services.AddSingleton<IInMemoryRepository<Hotel>, InMemoryHotelsRepository>();
        services.AddSingleton<AddBookingsAndHotelsToRepositoriesFacade>();
        services.AddSingleton<IDateProvider, DateProvider>();

        return services;
    }
}
