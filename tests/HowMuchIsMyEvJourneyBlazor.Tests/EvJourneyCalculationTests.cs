using Xunit;
using HowMuchIsMyEvJourneyBlazor.Models;

namespace HowMuchIsMyEvJourneyBlazor.Tests;

public class EvJourneyCalculationTests
{
    [Fact]
    public void Calculate_SimpleHomeCharging_CalculatesCorrectly()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 7.5,
            OnPeakTariffPencePerKwh = 15.0,
            BatterySizeKwh = 64.0,
            BatteryChargeStartPercent = 80.0,
            BatteryChargeEndPercent = 20.0,
            ChargedAtPublicInfrastructure = false,
            DistanceKm = 0,
            FuelEfficiencyMpg = 0,
            FuelCostPerLitre = 0
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(38.4, result.EnergyUsedKwh); // (80-20)/100 * 64 = 38.4 kWh
        Assert.Equal(38.4, result.EnergyFromHomeKwh);
        Assert.Equal(0, result.PublicChargingEnergyKwh);
        Assert.Equal(288.0, result.HomeChargeCostPence); // 38.4 * 7.5 = 288 pence
        Assert.Equal(0, result.PublicChargeCostPence);
        Assert.Equal(2.88, result.TotalCostPounds); // 288 / 100 = £2.88
        Assert.Equal(0, result.FuelCostPounds);
        Assert.Equal(0, result.SavingsPounds);
    }

    [Fact]
    public void Calculate_WithPublicCharging_CalculatesCorrectly()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 7.5,
            OnPeakTariffPencePerKwh = 15.0,
            BatterySizeKwh = 64.0,
            BatteryChargeStartPercent = 80.0,
            BatteryChargeEndPercent = 20.0,
            ChargedAtPublicInfrastructure = true,
            PublicChargingEnergyKwh = 20.0,
            PublicChargingCostPencePerKwh = 45.0
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(38.4, result.EnergyUsedKwh); // (80-20)/100 * 64 = 38.4 kWh
        Assert.Equal(18.4, result.EnergyFromHomeKwh); // 38.4 - 20 = 18.4 kWh
        Assert.Equal(20.0, result.PublicChargingEnergyKwh);
        Assert.Equal(138.0, result.HomeChargeCostPence); // 18.4 * 7.5 = 138 pence
        Assert.Equal(900.0, result.PublicChargeCostPence); // 20 * 45 = 900 pence
        Assert.Equal(10.38, result.TotalCostPounds); // (138 + 900) / 100 = £10.38
    }

    [Fact]
    public void Calculate_FullBatteryUsage_CalculatesCorrectly()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 10.0,
            OnPeakTariffPencePerKwh = 20.0,
            BatterySizeKwh = 50.0,
            BatteryChargeStartPercent = 100.0,
            BatteryChargeEndPercent = 0.0,
            ChargedAtPublicInfrastructure = false
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(50.0, result.EnergyUsedKwh); // (100-0)/100 * 50 = 50 kWh
        Assert.Equal(50.0, result.EnergyFromHomeKwh);
        Assert.Equal(500.0, result.HomeChargeCostPence); // 50 * 10 = 500 pence
        Assert.Equal(5.0, result.TotalCostPounds); // 500 / 100 = £5.00
    }

    [Fact]
    public void Calculate_PublicChargingExceedsUsage_HandlesCorrectly()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 7.5,
            OnPeakTariffPencePerKwh = 15.0,
            BatterySizeKwh = 64.0,
            BatteryChargeStartPercent = 80.0,
            BatteryChargeEndPercent = 20.0,
            ChargedAtPublicInfrastructure = true,
            PublicChargingEnergyKwh = 50.0, // More than total energy used
            PublicChargingCostPencePerKwh = 45.0
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(38.4, result.EnergyUsedKwh);
        Assert.Equal(0.0, result.EnergyFromHomeKwh); // Should be 0, not negative
        Assert.Equal(50.0, result.PublicChargingEnergyKwh);
        Assert.Equal(0.0, result.HomeChargeCostPence);
        Assert.Equal(2250.0, result.PublicChargeCostPence); // 50 * 45 = 2250 pence
        Assert.Equal(22.50, result.TotalCostPounds);
    }

    [Fact]
    public void Calculate_NegativeBatteryChange_ReturnsZero()
    {
        // Arrange - End battery higher than start
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 7.5,
            OnPeakTariffPencePerKwh = 15.0,
            BatterySizeKwh = 64.0,
            BatteryChargeStartPercent = 20.0,
            BatteryChargeEndPercent = 80.0, // Higher than start
            ChargedAtPublicInfrastructure = false
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(0.0, result.EnergyUsedKwh);
        Assert.Equal(0.0, result.EnergyFromHomeKwh);
        Assert.Equal(0.0, result.HomeChargeCostPence);
        Assert.Equal(0.0, result.TotalCostPounds);
    }

    [Fact]
    public void Calculate_SmallBattery_CalculatesCorrectly()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 5.0,
            OnPeakTariffPencePerKwh = 10.0,
            BatterySizeKwh = 30.0,
            BatteryChargeStartPercent = 50.0,
            BatteryChargeEndPercent = 25.0,
            ChargedAtPublicInfrastructure = false,
            DistanceKm = 0,
            FuelEfficiencyMpg = 0,
            FuelCostPerLitre = 0
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(7.5, result.EnergyUsedKwh); // (50-25)/100 * 30 = 7.5 kWh
        Assert.Equal(7.5, result.EnergyFromHomeKwh);
        Assert.Equal(37.5, result.HomeChargeCostPence); // 7.5 * 5 = 37.5 pence
        Assert.Equal(0.38, result.TotalCostPounds, precision: 2); // 37.5 / 100 = £0.375, rounded to £0.38
    }

    [Fact]
    public void Calculate_WithFuelComparison_CalculatesCorrectly()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 7.5,
            OnPeakTariffPencePerKwh = 15.0,
            BatterySizeKwh = 64.0,
            BatteryChargeStartPercent = 80.0,
            BatteryChargeEndPercent = 20.0,
            ChargedAtPublicInfrastructure = false,
            DistanceKm = 100.0,
            FuelEfficiencyMpg = 50.0,
            FuelCostPerLitre = 1.45
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        // EV costs
        Assert.Equal(38.4, result.EnergyUsedKwh);
        Assert.Equal(2.88, result.TotalCostPounds);
        
        // Fuel comparison: 100km = 62.14 miles, 62.14/50 = 1.24 gallons, 1.24 * 4.54609 = 5.64 litres, 5.64 * 1.45 = £8.19
        Assert.Equal(8.19, result.FuelCostPounds);
        
        // Savings: £8.19 - £2.88 = £5.31
        Assert.Equal(5.31, result.SavingsPounds);
    }

    [Fact]
    public void Calculate_WithZeroDistance_NoFuelComparison()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 7.5,
            OnPeakTariffPencePerKwh = 15.0,
            BatterySizeKwh = 64.0,
            BatteryChargeStartPercent = 80.0,
            BatteryChargeEndPercent = 20.0,
            ChargedAtPublicInfrastructure = false,
            DistanceKm = 0,
            FuelEfficiencyMpg = 50.0,
            FuelCostPerLitre = 1.45
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(0, result.FuelCostPounds);
        Assert.Equal(0, result.SavingsPounds);
    }

    [Fact]
    public void Calculate_WithLongDistance_CalculatesFuelCostCorrectly()
    {
        // Arrange
        var calculation = new EvJourneyCalculation
        {
            OffPeakTariffPencePerKwh = 7.5,
            OnPeakTariffPencePerKwh = 15.0,
            BatterySizeKwh = 64.0,
            BatteryChargeStartPercent = 100.0,
            BatteryChargeEndPercent = 0.0,
            ChargedAtPublicInfrastructure = false,
            DistanceKm = 500.0,
            FuelEfficiencyMpg = 45.0,
            FuelCostPerLitre = 1.50
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        // EV cost: 64 * 7.5 / 100 = £4.80
        Assert.Equal(4.80, result.TotalCostPounds);
        
        // Fuel cost: 500km = 310.69 miles, 310.69/45 = 6.90 gallons, 6.90 * 4.54609 = 31.37 litres, 31.37 * 1.50 = £47.08
        Assert.Equal(47.08, result.FuelCostPounds);
        
        // Savings
        Assert.Equal(42.28, result.SavingsPounds);
    }
}
