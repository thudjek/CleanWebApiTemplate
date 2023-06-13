using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Common.Exceptions;
public class IdentityException : Exception, IException
{
    public IdentityException() : base("Identity exception has occurred")
    {

    }

    public IdentityException(string message) : base(message)
    {

    }

    public IdentityException(IEnumerable<string> errors) : base("Identity exception has occurred")
    {
        Errors = errors;
    }

    public IdentityException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; }

    public void LogException(ILogger logger, string requestName) 
    {
        if (Errors is not null && Errors.Any())
        {
            logger.LogError(this, "Unhandeled exception of type IdentityException occurred while processing request {RequestName}. {@Errors}", requestName, new { Errors });
        }
        else
        {
            logger.LogError(this, "Unhandeled exception of type IdentityException occurred while processing request {RequestName}.", requestName);
        }
    }   
}