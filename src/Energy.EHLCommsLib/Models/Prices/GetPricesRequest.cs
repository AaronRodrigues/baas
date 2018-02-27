using System;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models.Prices
{
    public class GetPricesRequest : BaseRequest
    {
        public string CurrentSupplyUrl { get; set; }
        public string SwitchUrl { get; set; }
        public string SuppliersKey { get; set; }
        public bool UseDetailedEstimatorForElectricity { get; set; }
        public bool UseDetailedEstimatorForGas { get; set; }
        public string PrePayment { get; set; }
        public CompareWhat CompareType { get; set; }
        public UsageData UsageData { get; } = new UsageData();
        public SpendData SpendData { get; } = new SpendData();
        public EstimatorData EstimatorData { get; } = new EstimatorData();
        public int GasSupplierId { get; set; }
        public string GasSupplierName { get; set; }
        public int GasTariffId { get; set; }
        public string GasTariffName { get; set; }
        public string GasTariffType { get; set; }
        public int GasPaymentMethodId { get; set; }
        public string GasPaymentMethodName { get; set; }
        public int ElectricitySupplierId { get; set; }
        public string ElectricitySupplierName { get; set; }
        public string ElectricityTariffType { get; set; }
        public int ElectricityTariffId { get; set; }
        public string ElectricityTariffName { get; set; }
        public int ElectricityPaymentMethodId { get; set; }
        public string ElectricityPaymentMethodName { get; set; }
        public bool ElectricityEco7 { get; set; }
        public decimal PercentageNightUsage { get; set; }
        public int Eco7DayUsageValue { get; set; }
        public int Eco7NightUsageValue { get; set; }
        public int DefaultElectricitySupplierId { get; set; }

        public bool CalculateElecBasedOnBillSpend => SpendData.ElectricitySpendAmount > 0;

        public bool CalculateGasBasedOnBillSpend => SpendData.GasSpendAmount > 0;

        public Guid JourneyId { get; set; }
        public bool IgnoreProRataComparison { get; set; }
    }
}