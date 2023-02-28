using Application.Common.Interfaces;
using Application.Common;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";

            var errorModel = new ErrorModel();
            context.Response.StatusCode = 500;

            if (ex is IException exception)
            {
                errorModel = exception.ToErrorModel();
                context.Response.StatusCode = (int)exception.StatusCode;
            }

            await context.Response.WriteAsync(errorModel.ToJson());
        }
    }
}