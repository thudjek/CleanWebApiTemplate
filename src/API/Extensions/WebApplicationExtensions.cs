using Application;
using Application.Common;
using Application.Common.Exceptions;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

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

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();

        app.UseExceptionHandling();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(ctx.Configuration["Elastic:Uri"]))
            {
                IndexFormat = $"Test-logs-local-{{0:yyyy.MM.dd}}",

                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                NumberOfShards = 1,
                NumberOfReplicas = 1
            })
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }

    private static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(configure =>
        {
            configure.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                context.Response.ContentType = "application/json";

                var errorModel = new ErrorModel();

                switch (exceptionHandlerPathFeature?.Error)
                {
                    case ValidationException ex:
                        errorModel.Error = ex.Message;
                        errorModel.Errors = ex.Errors;
                        errorModel.ErrorsGrouped = ex.ErrorsGrouped;
                        context.Response.StatusCode = 400;
                        break;
                    case NotFoundException ex:
                        if (!string.IsNullOrWhiteSpace(ex.UserMessage))
                            errorModel.Error = ex.UserMessage;
                        context.Response.StatusCode = 404;
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        break;
                }

                await context.Response.WriteAsync(errorModel.ToJson());
            });
        });

        return app;
    }
}
