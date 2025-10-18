using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EvJourneyCalculator;

public class CalculateJourneyCost
{
    private readonly ILogger<CalculateJourneyCost> _logger;

    public CalculateJourneyCost(ILogger<CalculateJourneyCost> logger)
    {
        _logger = logger;
    }

    [Function("CalculateJourneyCost")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "journey/calculate")] HttpRequest req)
    {
        _logger.LogInformation("Processing EV journey cost calculation request");

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<JourneyRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return new BadRequestObjectResult(new { error = "Invalid request body" });
            }

            if (data.DistanceKm <= 0)
            {
                return new BadRequestObjectResult(new { error = "Distance must be greater than 0" });
            }

            if (data.ElectricityRatePerKwh <= 0)
            {
                return new BadRequestObjectResult(new { error = "Electricity rate must be greater than 0" });
            }

            if (data.VehicleEfficiencyKwhPer100Km <= 0)
            {
                return new BadRequestObjectResult(new { error = "Vehicle efficiency must be greater than 0" });
            }

            var energyUsedKwh = (data.DistanceKm / 100) * data.VehicleEfficiencyKwhPer100Km;
            var totalCost = energyUsedKwh * data.ElectricityRatePerKwh;

            var response = new JourneyResponse
            {
                DistanceKm = data.DistanceKm,
                ElectricityRatePerKwh = data.ElectricityRatePerKwh,
                VehicleEfficiencyKwhPer100Km = data.VehicleEfficiencyKwhPer100Km,
                EnergyUsedKwh = Math.Round(energyUsedKwh, 2),
                TotalCost = Math.Round(totalCost, 2),
                Currency = data.Currency ?? "USD"
            };

            _logger.LogInformation("Journey cost calculated successfully: {TotalCost} {Currency}", 
                response.TotalCost, response.Currency);

            return new OkObjectResult(response);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new { error = "Invalid JSON format" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating journey cost");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

public class JourneyRequest
{
    public double DistanceKm { get; set; }
    public double ElectricityRatePerKwh { get; set; }
    public double VehicleEfficiencyKwhPer100Km { get; set; }
    public string? Currency { get; set; }
}

public class JourneyResponse
{
    public double DistanceKm { get; set; }
    public double ElectricityRatePerKwh { get; set; }
    public double VehicleEfficiencyKwhPer100Km { get; set; }
    public double EnergyUsedKwh { get; set; }
    public double TotalCost { get; set; }
    public string Currency { get; set; } = "USD";
}
