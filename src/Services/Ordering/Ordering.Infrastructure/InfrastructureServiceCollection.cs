using GreatIdeas.MailServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Infrastructure.Mail;
using Ordering.Infrastructure.Respositories;

namespace Ordering.Infrastructure;

public enum DbProviders
{
    Postgres = 1,
    MariaDb,
}

public static class InfrastructureServiceCollection
{
    private static DbProviders DbProvider { get; set; } = DbProviders.Postgres;

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // register repositories
        services.AddScoped<IOrderRepository, OrderRepository>();

        // register email service
        services.Configure<EmailSettings>(c => configuration.GetSection(nameof(EmailSettings)));
        services.AddMsGraphMailService(configuration);
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}