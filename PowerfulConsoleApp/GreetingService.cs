using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class GreetingService : IGreetingService
{
    private readonly ILogger<GreetingService> _logger;
    private readonly IConfiguration _config;

    public GreetingService(ILogger<GreetingService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public void Run()
    {
        for (int i = 0; i < _config.GetValue
            <int>("LoopTimes"); i++)
        {
            _logger.LogInformation("Run number {number}", i);
        }
    }
}