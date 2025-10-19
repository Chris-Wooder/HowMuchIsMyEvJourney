namespace HowMuchIsMyEvJourneyBlazor.Models;

public enum DistanceUnit
{
    Kilometers,
    Miles
}

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
    public double Distance { get; set; }
    public DistanceUnit DistanceUnit { get; set; }
    public double DistanceKm { get; set; }
    public double FuelEfficiencyMpg { get; set; }
    public double FuelCostPerLitre { get; set; }

    public EvJourneyResult Calculate()
    {
        // Convert distance to kilometers for internal calculations
        var distanceInKm = DistanceUnit == DistanceUnit.Miles ? Distance * 1.60934 : Distance;
        
        // Support legacy DistanceKm property
        if (Distance == 0 && DistanceKm > 0)
        {
            distanceInKm = DistanceKm;
        }
        
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

        // Calculate fuel comparison cost
        var fuelCostPounds = 0.0;
        if (distanceInKm > 0 && FuelEfficiencyMpg > 0 && FuelCostPerLitre > 0)
        {
            // Convert distance from km to miles
            var distanceMiles = distanceInKm * 0.621371;
            
            // Calculate fuel needed in gallons
            var gallonsNeeded = distanceMiles / FuelEfficiencyMpg;
            
            // Convert gallons to litres (1 gallon = 4.54609 litres for UK gallon)
            var litresNeeded = gallonsNeeded * 4.54609;
            
            // Calculate fuel cost
            fuelCostPounds = litresNeeded * FuelCostPerLitre;
        }

        return new EvJourneyResult
        {
            EnergyUsedKwh = Math.Round(energyUsedKwh, 2),
            EnergyFromHomeKwh = Math.Round(energyFromHomeKwh, 2),
            PublicChargingEnergyKwh = ChargedAtPublicInfrastructure ? Math.Round(PublicChargingEnergyKwh, 2) : 0,
            HomeChargeCostPence = Math.Round(homeChargeCostPence, 2),
            PublicChargeCostPence = Math.Round(publicChargeCostPence, 2),
            TotalCostPounds = Math.Round(totalCostPounds, 2),
            FuelCostPounds = Math.Round(fuelCostPounds, 2),
            SavingsPounds = fuelCostPounds > 0 ? Math.Round(fuelCostPounds - totalCostPounds, 2) : 0
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
    public double FuelCostPounds { get; set; }
    public double SavingsPounds { get; set; }
}
