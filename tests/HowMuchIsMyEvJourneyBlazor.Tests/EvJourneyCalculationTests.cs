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
            ChargedAtPublicInfrastructure = false
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
            ChargedAtPublicInfrastructure = false
        };

        // Act
        var result = calculation.Calculate();

        // Assert
        Assert.Equal(7.5, result.EnergyUsedKwh); // (50-25)/100 * 30 = 7.5 kWh
        Assert.Equal(7.5, result.EnergyFromHomeKwh);
        Assert.Equal(37.5, result.HomeChargeCostPence); // 7.5 * 5 = 37.5 pence
        Assert.Equal(0.38, result.TotalCostPounds, precision: 2); // 37.5 / 100 = £0.375, rounded to £0.38
    }
}
