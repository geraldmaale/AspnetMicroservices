using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Data;

namespace Ordering.API.Extensions;

public enum DbProviders
{
    Postgres = 1,
    MariaDb,
}

public static class DbContextServiceCollection
{
    private static DbProviders DbProvider { get; set; } = DbProviders.Postgres;

    public static IServiceCollection AddDbContextService(this IServiceCollection services,
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
                    options.UseNpgsql(dbConnectionString, x => x.MigrationsAssembly("Ordering.Infrastructure"))
                        .EnableSensitiveDataLogging()
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Information));
            }
            else
            {
                services.AddDbContextFactory<OrderDbContext>(options =>
                    options.UseNpgsql(dbConnectionString, x => x.MigrationsAssembly("Ordering.Infrastructure"))
                        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                            LogLevel.Error));
            }
        }

        services.AddScoped(p =>
            p.GetRequiredService<IDbContextFactory<OrderDbContext>>()
                .CreateDbContext());

        #endregion

        return services;
    }
}