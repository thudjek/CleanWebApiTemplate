namespace CleanWebApiTemplate.Application.Common.Interfaces;
public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
