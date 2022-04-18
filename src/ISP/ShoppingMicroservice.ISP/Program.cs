using Core.Logging;
using Serilog;
using ShoppingMicroservice.ISP;

// Logging boostrap
LoggingServiceCollection.AddSerilogBootstrapLogging();


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Logging
    builder.AddLoggingServices();

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    // this seeding is only for the template to bootstrap the DB and users.
    // in production you will likely want a different approach.
    if (builder.Environment.IsDevelopment())
    {
        Log.Information("Seeding database...");
        SeedData.EnsureSeedData(app);
        Log.Information("Done seeding database. Exiting");
    }

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException") // https://github.com/dotnet/runtime/issues/60600
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}