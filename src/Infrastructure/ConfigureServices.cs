using Application.Common.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;

namespace Infrastructure;
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
#if SQLServer
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration["Database:ConnectionString"], b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

#endif
#if PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration["Database:ConnectionString"], b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

#endif
        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<DatabaseInitializer>();

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.AllowedForNewUsers = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddSendGrid(options =>
        {
            options.ApiKey = configuration["SendGrid:ApiKey"];
        });

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddTransient<IEmailService, SendGridEmailService>();

        return services;
    }
}
