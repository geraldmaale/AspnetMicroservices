using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using SerilogTimings;

namespace Ordering.API.Extensions;

public static class LoggingServiceCollection
{
    public static void AddLoggingServices(this WebApplicationBuilder builder)
    {
        Log.Information("Starting web host ...");
       

        // Full setup of serilog logging
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, services, configuration) => configuration
            // .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .WriteTo.Console()
            .WriteTo.Seq(builder.Configuration["SeqConfiguration:Uri"])
            .WriteTo.File("logs/orderingapi-logs.log", rollingInterval: RollingInterval.Day)
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"])) {
                AutoRegisterTemplate = true,
                NumberOfShards = 2,
                IndexFormat =
                    $"{builder.Configuration["ApplicationName"]}-logs-{builder.Environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM-dd}"
            })
            .WriteTo.ApplicationInsights(builder.Services.BuildServiceProvider().GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces, LogEventLevel.Information)
            .WriteTo.ApplicationInsights(builder.Services.BuildServiceProvider().GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Events, LogEventLevel.Information)
        );

        // Application Insights
        builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
    }
}