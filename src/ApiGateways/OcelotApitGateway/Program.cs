 using Core.Logging;
 using Ocelot.DependencyInjection;
 using Ocelot.Middleware;

 var builder = WebApplication.CreateBuilder(args);
 
 // Configure Logging
 builder.AddLoggingServices();
 
 // Ocelot Gateway services
 builder.Services.AddOcelot();
 
var app = builder.Build();
 
 // Use Logging
 app.UseLoggingMiddleware();
 
 app.MapGet("/", () => "Hello World!");

 
 // Use Ocelot
 await app.UseOcelot();

app.Run();