using System;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models.Prices
{
    public class GetPricesRequest : BaseRequest
    {
        private readonly UsageData _usageData = new UsageData();
        private readonly SpendData _spendData = new SpendData();
        private readonly EstimatorData _estimatorData = new EstimatorData();

        public string CurrentSupplyUrl { get; set; }
        public string SwitchUrl { get; set; }
        public string SuppliersKey { get; set; }

        public bool UseDetailedEstimatorForElectricity { get; set; }
        public bool UseDetailedEstimatorForGas { get; set; }

        public string PrePayment { get; set; }

        public CompareWhat CompareType { get; set; }

        public UsageData UsageData { get { return _usageData; } }
        public SpendData SpendData { get { return _spendData; } }
        public EstimatorData EstimatorData { get { return _estimatorData; } }

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

        public bool CalculateElecBasedOnBillSpend
        {
            get { return SpendData.ElectricitySpendAmount > 0; }
        }
        public bool CalculateGasBasedOnBillSpend
        {
            get { return SpendData.GasSpendAmount > 0; }
        }

        public Guid JourneyId { get; set; }

        public bool IgnoreProRataComparison { get; set; }
    }
}
