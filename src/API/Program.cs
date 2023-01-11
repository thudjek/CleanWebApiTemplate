using API.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    WebApplication.CreateBuilder(args)
        .ConfigureBuilderAndServices()
        .Build()
        .ConfigureApplication()
        .Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to start");
}
finally
{
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}
