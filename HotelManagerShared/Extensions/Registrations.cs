using HotelManager.Shared.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManager.Shared.Extensions;

public static class Registrations
{
    public static IServiceCollection RegisterSharedServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AvailabilityCommand>());
        return services;
    }
}
