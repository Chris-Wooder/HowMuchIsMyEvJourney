using Xunit;
using EvJourneyCalculator;

namespace EvJourneyCalculator.Tests;

public class JourneyCalculationTests
{
    [Fact]
    public void JourneyRequest_ValidData_CalculatesCorrectly()
    {
        // Arrange
        var request = new JourneyRequest
        {
            DistanceKm = 100,
            ElectricityRatePerKwh = 0.15,
            VehicleEfficiencyKwhPer100Km = 18,
            Currency = "USD"
        };

        // Act
        var energyUsed = (request.DistanceKm / 100) * request.VehicleEfficiencyKwhPer100Km;
        var totalCost = energyUsed * request.ElectricityRatePerKwh;

        // Assert
        Assert.Equal(18, energyUsed);
        Assert.Equal(2.7, totalCost, precision: 2);
    }

    [Fact]
    public void JourneyRequest_ShortDistance_CalculatesCorrectly()
    {
        // Arrange
        var request = new JourneyRequest
        {
            DistanceKm = 50,
            ElectricityRatePerKwh = 0.20,
            VehicleEfficiencyKwhPer100Km = 15,
            Currency = "EUR"
        };

        // Act
        var energyUsed = (request.DistanceKm / 100) * request.VehicleEfficiencyKwhPer100Km;
        var totalCost = energyUsed * request.ElectricityRatePerKwh;

        // Assert
        Assert.Equal(7.5, energyUsed);
        Assert.Equal(1.5, totalCost);
    }

    [Fact]
    public void JourneyRequest_LongDistance_CalculatesCorrectly()
    {
        // Arrange
        var request = new JourneyRequest
        {
            DistanceKm = 500,
            ElectricityRatePerKwh = 0.12,
            VehicleEfficiencyKwhPer100Km = 20,
            Currency = "GBP"
        };

        // Act
        var energyUsed = (request.DistanceKm / 100) * request.VehicleEfficiencyKwhPer100Km;
        var totalCost = energyUsed * request.ElectricityRatePerKwh;

        // Assert
        Assert.Equal(100, energyUsed);
        Assert.Equal(12, totalCost);
    }

    [Fact]
    public void JourneyResponse_DefaultCurrency_IsUSD()
    {
        // Arrange & Act
        var response = new JourneyResponse();

        // Assert
        Assert.Equal("USD", response.Currency);
    }
}
