using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Interceptors;
public class AuditEntitiesInterceptor : SaveChangesInterceptor
{
	private readonly ICurrentUserService _currentUserService;
	private readonly IDateTimeService _dateTimeService;
    public AuditEntitiesInterceptor(ICurrentUserService currentUserService, IDateTimeService dateTimeService)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AuditEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AuditEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AuditEntities(DbContext context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _currentUserService.UserId;
                entry.Entity.DateCreated = _dateTimeService.Now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedBy = _currentUserService.UserId;
                entry.Entity.DateModified = _dateTimeService.Now;
            }
        }
    }
}