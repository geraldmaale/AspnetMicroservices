using System.Reflection;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Behaviours;

namespace Ordering.Application;

public static class ApplicationServiceCollections
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Mapster Mapping
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        // Register validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register MediatR
        services.AddMediatR(Assembly.GetExecutingAssembly());

        // Register MediatR pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

        return services;
    }
}