using AppName.Infrastructure.Exceptions;
using AppName.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppName.Infrastructure.Persistence;
public class DatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    public DatabaseInitializer(ILogger<DatabaseInitializer> logger, AppDbContext dbContext, UserManager<User> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task InitializeDatabase()
    {
        try
        {
            var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await _dbContext.Database.MigrateAsync();
            }
            else if (appliedMigrations.ToList().Count == 0)
            {
                throw new MigrationException("Database cannot start without at least one migration");
            }
        }
        catch (MigrationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while initializig database");
            throw;
        }
    }

    public async Task SeedDatabase()
    {
        try
        {
            await SeedUsers();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while seeding database");
            throw;
        }
    }

    private async Task SeedUsers()
    {
        var user = new User()
        {
            Email = "test@test.com",
            UserName = "test@test.com",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != user.Email))
        {
            await _userManager.CreateAsync(user, "ASDqwe123");
        }
    }
}