using CleanWebApiTemplate.Application.Common.Interfaces;

namespace CleanWebApiTemplate.Infrastructure.Services;
public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
