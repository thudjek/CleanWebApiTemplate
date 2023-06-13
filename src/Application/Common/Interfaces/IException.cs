using Microsoft.Extensions.Logging;

namespace Application.Common.Interfaces;
public interface IException
{
    void LogException(ILogger logger, string requestName);
}