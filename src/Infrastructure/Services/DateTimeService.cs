﻿using Application.Common.Interfaces;

namespace Infrastructure.Services;
public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
