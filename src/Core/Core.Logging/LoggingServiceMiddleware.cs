using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Core.Logging;

public static class LoggingServiceMiddleware
{
    public static void UseLoggingMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging(config => {
            config.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} {UserId} responded {StatusCode} in {Elapsed:0.0000} ms";
        });
    }
}
