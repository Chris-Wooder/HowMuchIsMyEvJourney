using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EvJourneyCalculator;

public class HealthCheck
{
    private readonly ILogger<HealthCheck> _logger;

    public HealthCheck(ILogger<HealthCheck> logger)
    {
        _logger = logger;
    }

    [Function("HealthCheck")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
    {
        _logger.LogInformation("Health check endpoint called");

        return new OkObjectResult(new
        {
            status = "Healthy",
            service = "EV Journey Calculator",
            timestamp = DateTime.UtcNow
        });
    }
}
