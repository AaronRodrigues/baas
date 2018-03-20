using System;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models.Prices
{

    public class GetPricesRequest : BaseRequest
    {
        //todo refactor: based on calculus
        /*
                private bool UseDetailedEstimatorForElectricity
                {
                    get
                    {
                        if (ComparingGasOnly)
                            return false;

                        return NoBillViewModel.ElectricitySpendDontKnow;
                    }
                }

                private bool UseDetailedEstimatorForGas
                {
                    get
                    {
                        if (ComparingElectricityOnly)
                            return false;

                        return NoBillViewModel.GasSpendDontKnow;
                    }
                }
        */
        public bool UseDetailedEstimatorForElectricity { get; set; }
        public bool UseDetailedEstimatorForGas { get; set; }

        public UsageData UsageData { get; } = new UsageData();
        public SpendData SpendData { get; } = new SpendData();
        public EstimatorData EstimatorData { get; } = new EstimatorData();

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
    }
}