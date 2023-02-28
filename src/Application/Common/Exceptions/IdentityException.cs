using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Common.Exceptions;
public class IdentityException : Exception, IException
{
    public IdentityException() : base("Identity exception has occurred")
    {

    }

    public IdentityException(IEnumerable<string> errors) : base("Identity exception has occurred")
    {
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; }

    public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;

    public ErrorModel ToErrorModel() => new();

    public void LogException(ILogger logger, string requestName) =>
        logger.LogError(this, "Unhandeled exception of type IdentityException occurred while processing request {RequestName}. {@Errors}", requestName, new { Errors });
}