using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppName.Application.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonOptionsWithStartupValidation<TOptions>(this IServiceCollection services, IConfiguration configurationSection) where TOptions : class
    {
        services.AddOptions<TOptions>()
            .Bind(configurationSection)
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }
}