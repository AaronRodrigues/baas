using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models;

namespace Energy.ProviderAdapter.Models
{
    public class EnergyEnquiry
    {
        public Guid JourneyId { get; set; }
        public bool UseDetailedEstimatorForElectricity { get; set; }
        public bool UseDetailedEstimatorForGas { get; set; }
        public UsageData UsageData { get; } = new UsageData();
        public SpendData SpendData { get; } = new SpendData();
        public EstimatorData EstimatorData { get; } = new EstimatorData();
        public bool IgnoreProRataComparison { get; set; }
        public string CurrentSupplyUrl { get; set; }
        public string SwitchUrl { get; set; }
        public string PrePayment { get; set; }
        public CompareWhat CompareType { get; set; }
        public int GasSupplierId { get; set; }
        public int GasTariffId { get; set; }
        public int GasPaymentMethodId { get; set; }
        public int ElectricitySupplierId { get; set; }
        public int ElectricityTariffId { get; set; }
        public int ElectricityPaymentMethodId { get; set; }
        public bool ElectricityEco7 { get; set; }
        public decimal PercentageNightUsage { get; set; }
        public bool TariffCustomFeatureEnabled { get; set; }
        public Dictionary<string,string> CustomFeatures { get; set; }
    }
}
