using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Common.Interfaces;
public interface IException
{
    public HttpStatusCode StatusCode { get; }
    ErrorModel ToErrorModel();
    void LogException(ILogger logger, string requestName);
}