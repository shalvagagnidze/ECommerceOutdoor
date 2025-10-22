using DomainLayer.Interfaces;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<ECommerceDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ECommerceOutdoor"));
        },
        ServiceLifetime.Scoped);

        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
