using API.Middleware;
using Application;
using Infrastructure;
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
            app.UseSwagger();
            app.UseSwaggerUI();
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

    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }
}
