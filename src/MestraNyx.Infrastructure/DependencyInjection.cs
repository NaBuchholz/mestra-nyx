using MestraNyx.Domain.Interfaces;
using MestraNyx.Infrastructure.Persistence;
using MestraNyx.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MestraNyx.Infrastructure;

/// <summary>
/// Provides dependency injection registrations for infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds persistence and repository services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection used to register dependencies.</param>
    /// <param name="connectionString">The database connection string used by the persistence layer.</param>
    /// <returns>The same service collection so that additional registrations can be chained.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICampaignRepository, CampaignRepository>();

        return services;
    }
}
