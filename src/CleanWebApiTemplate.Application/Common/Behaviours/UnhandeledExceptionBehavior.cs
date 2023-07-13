using CleanWebApiTemplate.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanWebApiTemplate.Application.Common.Behaviours;

public class UnhandeledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
{
    private readonly ILogger<UnhandeledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandeledExceptionBehavior(ILogger<UnhandeledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var exceptionType = ex.GetType();
            
            if (typeof(IException).IsAssignableFrom(exceptionType))
            {
                var iex = ex as IException;
                iex.LogException(_logger, requestName);
            }
            else
            {
                _logger.LogError(ex, "Unhandeled exception of type {ExceptionType} occurred while processing request {RequestName}.", exceptionType.Name, requestName);
            }

            throw;
        }
    }
}
