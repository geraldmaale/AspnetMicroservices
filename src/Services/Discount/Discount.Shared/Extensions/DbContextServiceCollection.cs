using Discount.Shared.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discount.Shared.Extensions;

public enum DbProviders
{
    Postgres = 1,
    MssqlServer,
}

public static class DbContextServiceCollection
{
    private static DbProviders DbProvider { get; set; } = DbProviders.Postgres;

    public static IServiceCollection RegisterDbContextServiceCollection(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (DbProvider == DbProviders.Postgres)
        {
            var dbConnectionString = configuration["DatabaseSettings:ConnectionString"];
            Console.WriteLine(dbConnectionString);

            // register factory and configure the options
            if (environment.IsDevelopment())
            {
                services.AddDbContextFactory<DiscountDbContext>(options =>
                    options.UseNpgsql(dbConnectionString)
                        .EnableSensitiveDataLogging()
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Information));
            }
            else
            {
                services.AddDbContextFactory<DiscountDbContext>(options =>
                    options.UseNpgsql(dbConnectionString)
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Error));
            }
        }

        services.AddScoped(p =>
            p.GetRequiredService<IDbContextFactory<DiscountDbContext>>()
                .CreateDbContext());

        return services;
    }
}