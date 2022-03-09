using Discount.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Discount.API.Extensions;

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
            var dbConnectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");

            // register factory and configure the options
            if (environment.IsDevelopment())
            {
                services.AddDbContextFactory<ApplicationDbContext>(options =>
                    options.UseNpgsql(dbConnectionString)
                        .EnableSensitiveDataLogging()
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Information));
            }
            else
            {
                services.AddDbContextFactory<ApplicationDbContext>(options =>
                    options.UseNpgsql(dbConnectionString)
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Error));
            }
        }

        services.AddScoped(p =>
            p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                .CreateDbContext());

        return services;
    }
}