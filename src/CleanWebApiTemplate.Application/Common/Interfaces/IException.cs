using Microsoft.Extensions.Logging;

namespace CleanWebApiTemplate.Application.Common.Interfaces;
public interface IException
{
    void LogException(ILogger logger, string requestName);
}