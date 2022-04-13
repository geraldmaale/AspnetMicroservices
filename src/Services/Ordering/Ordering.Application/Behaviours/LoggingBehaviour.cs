using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        //Request
        _logger.LogInformation("Handling {TRequest}", typeof(TRequest).Name);
        Type myType = request.GetType();
        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
        foreach (var prop in props)
        {
            var propValue = prop.GetValue(request, null);
            _logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
        }
        
        //Response
        var response = await next();       
        _logger.LogInformation("Handled {TResponse}", typeof(TResponse).Name);
        
        return response;
    }
}