namespace HowMuchIsMyEvJourneyBlazor.Models;

public class EvJourneyCalculation
{
    public double OffPeakTariffPencePerKwh { get; set; }
    public double OnPeakTariffPencePerKwh { get; set; }
    public double BatteryChargeStartPercent { get; set; }
    public double BatterySizeKwh { get; set; }
    public double BatteryChargeEndPercent { get; set; }
    public bool ChargedAtPublicInfrastructure { get; set; }
    public double PublicChargingEnergyKwh { get; set; }
    public double PublicChargingCostPencePerKwh { get; set; }

    public EvJourneyResult Calculate()
    {
        var energyUsedKwh = ((BatteryChargeStartPercent - BatteryChargeEndPercent) / 100.0) * BatterySizeKwh;
        
        if (energyUsedKwh < 0)
        {
            energyUsedKwh = 0;
        }

        var energyFromHomeKwh = energyUsedKwh;
        var publicChargeCostPence = 0.0;
        
        if (ChargedAtPublicInfrastructure && PublicChargingEnergyKwh > 0)
        {
            energyFromHomeKwh = Math.Max(0, energyUsedKwh - PublicChargingEnergyKwh);
            publicChargeCostPence = PublicChargingEnergyKwh * PublicChargingCostPencePerKwh;
        }

        var homeChargeCostPence = energyFromHomeKwh * OffPeakTariffPencePerKwh;
        var totalCostPence = homeChargeCostPence + publicChargeCostPence;
        var totalCostPounds = totalCostPence / 100.0;

        return new EvJourneyResult
        {
            EnergyUsedKwh = Math.Round(energyUsedKwh, 2),
            EnergyFromHomeKwh = Math.Round(energyFromHomeKwh, 2),
            PublicChargingEnergyKwh = ChargedAtPublicInfrastructure ? Math.Round(PublicChargingEnergyKwh, 2) : 0,
            HomeChargeCostPence = Math.Round(homeChargeCostPence, 2),
            PublicChargeCostPence = Math.Round(publicChargeCostPence, 2),
            TotalCostPounds = Math.Round(totalCostPounds, 2)
        };
    }
}

public class EvJourneyResult
{
    public double EnergyUsedKwh { get; set; }
    public double EnergyFromHomeKwh { get; set; }
    public double PublicChargingEnergyKwh { get; set; }
    public double HomeChargeCostPence { get; set; }
    public double PublicChargeCostPence { get; set; }
    public double TotalCostPounds { get; set; }
}
