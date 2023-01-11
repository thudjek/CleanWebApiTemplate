using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;
public interface IAppDbContext
{
    DbSet<TestEntity> TestEntities { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
