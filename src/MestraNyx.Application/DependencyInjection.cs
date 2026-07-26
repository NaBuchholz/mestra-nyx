using FluentValidation;
using MestraNyx.Application.Campaigns.Commands.CreateCampaigns;
using MestraNyx.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace MestraNyx.Application;

/// <summary>
/// Provides dependency injection registrations for application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application use cases and validation services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection used to register dependencies.</param>
    /// <returns>The same service collection so that additional registrations can be chained.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly);

            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<
            IValidator<CreateCampaignCommand>,
            CreateCampaignValidator>();

        return services;
    }
}
