using Microsoft.Extensions.Logging;

namespace AppName.Application.Common.Interfaces;
public interface IException
{
    void LogException(ILogger logger, string requestName);
}