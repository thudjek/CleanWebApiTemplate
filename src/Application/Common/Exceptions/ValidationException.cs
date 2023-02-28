using Application.Common.Interfaces;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Common.Exceptions;
public class ValidationException : Exception, IException
{
    public ValidationException() : base("One or more validations errors has occurred.")
    {

    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ValidationErrors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

    }
    public Dictionary<string, string[]> ValidationErrors { get; }

    public HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;

    public ErrorModel ToErrorModel() => new(Message, ValidationErrors);

    public void LogException(ILogger logger, string requestName)
    {
    }
}