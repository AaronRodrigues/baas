using Energy.EHLCommsLib.Models;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class StartSwitchResponse : BaseResponse
    {
        public bool CompareGas { get; set; }
        public bool CompareElectricity { get; set; }

        public int DefaultGasSupplierId { get; set; }
        public int DefaultGasSupplierTariffId { get; set; }
        public string DefaultGasPaymentMethod { get; set; }

        public int DefaultElecSupplierId { get; set; }
        public int DefaultElecSupplierTariffId { get; set; }
        public string DefaultElecPaymentMethod { get; set; }
        public bool DefaultElecEconomy7 { get; set; }

        public string PostCode { get; set; }
        public string SwitchId { get; set; }
        public string SwitchStatusUrl { get; set; }
        public string CurrentSupplyUrl { get; set; }
        public string CurrentSuppliersUrl { get; set; }
    }
}
