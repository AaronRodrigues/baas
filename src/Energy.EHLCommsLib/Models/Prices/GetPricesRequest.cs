using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models.Prices
{

    public class GetPricesRequest : BaseRequest
    {
        public GetPricesRequest()
        {
            UsageData = new UsageData();
            SpendData = new SpendData();
            EstimatorData = new EstimatorData();
            CustomFeatures = new Dictionary<string, string>();
        }

        public bool UseDetailedEstimatorForElectricity { get; set; }
        public bool UseDetailedEstimatorForGas { get; set; }
        public UsageData UsageData { get; } 
        public SpendData SpendData { get; set; }
        public EstimatorData EstimatorData { get; } 
        public bool CalculateElecBasedOnBillSpend => SpendData.ElectricitySpendAmount > 0;
        public bool CalculateGasBasedOnBillSpend => SpendData.GasSpendAmount > 0;
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
        public Guid JourneyId { get; set; }
        public Dictionary<string, string> CustomFeatures { get; set; }
        public bool TariffCustomFeatureEnabled { get; set; }
    }
}