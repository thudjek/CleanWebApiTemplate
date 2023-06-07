using API.Middleware;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Serilog.Events;

namespace API.Extensions;
public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureBuilderAndServices(this WebApplicationBuilder builder)
    {
        builder.AddSerilog();
        
        builder.Services
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddAPIServices(builder.Configuration);

        return builder;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            IApiVersionDescriptionProvider provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(options => 
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"{app.Configuration["API:Name"]} {description.GroupName}");
                }

                options.DefaultModelsExpandDepth(-1);
            });

            app.InitilazeDatabaseForDevelopment().Wait();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static async Task<WebApplication> InitilazeDatabaseForDevelopment(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await databaseInitializer.InitializeDatabase();
            await databaseInitializer.SeedAsync();
        }

        return app;
    }

    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }
}
