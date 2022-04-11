using GreatIdeas.MailServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Mail;
using Ordering.Infrastructure.Respositories;

namespace Ordering.Infrastructure;

public enum DbProviders
{
    Postgres = 1,
    MssqlServer,
}

public static class InfrastructureServiceCollection
{
    private static DbProviders DbProvider { get; set; } = DbProviders.Postgres;

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment environment)
    {
        // register the OrderContext
        #region DbContext

        if (DbProvider == DbProviders.Postgres)
        {
            var dbConnectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");

            // register factory and configure the options
            if (environment.IsDevelopment())
            {
                services.AddDbContextFactory<OrderDbContext>(options =>
                    options.UseNpgsql(dbConnectionString)
                        .EnableSensitiveDataLogging()
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Information));
            }
            else
            {
                services.AddDbContextFactory<OrderDbContext>(options =>
                    options.UseNpgsql(dbConnectionString)
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Error));
            }
        }

        services.AddScoped(p =>
            p.GetRequiredService<IDbContextFactory<OrderDbContext>>()
                .CreateDbContext());

        #endregion
        
        // register repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        
        // register email service
        services.Configure<EmailSettings>(c=> configuration.GetSection(nameof(EmailSettings))); 
        services.AddMsGraphMailService(configuration);
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}