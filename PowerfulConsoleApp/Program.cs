using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = new ConfigurationBuilder();
BuildConfig(builder);

var config = builder.Build();

Log.Logger = new LoggerConfiguration().
    ReadFrom.Configuration(config).
    Enrich.FromLogContext().
    Enrich.WithMemoryUsage().
    Enrich.WithThreadId().

    // For basic output use 'WriteTo.Console()'.
    // Fancy output format
    WriteTo.Console(theme: AnsiConsoleTheme.Sixteen, outputTemplate: "{Timestamp:yyyy/MM/dd HH:mm} [{Level:u3}] (Thread: {ThreadId}) (Mem: {MemoryUsage}) {Message}{NewLine}{Exception}").
    CreateLogger();

Log.Logger.Information("App starting");

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<IGreetingService, GreetingService>();
    })
    .UseSerilog()
    .Build();

// Not that good way to instatiate in this case, because doesn't use the dependency injection
// Better way Ex: https://github.com/evaldosilva/ConsoleDependencyInjectionApp/blob/844bef71c3336bb4feaaeb42c67e8b8b534c9149/ConsoleDependencyInjectionApp/Program.cs#L42
var svc = ActivatorUtilities.CreateInstance<GreetingService>(host.Services);
svc.Run();

static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings{Environment.GetEnvironmentVariable("ASPNETCORE>ENVIRONMENT") ?? "Production"}.json",
        optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
}