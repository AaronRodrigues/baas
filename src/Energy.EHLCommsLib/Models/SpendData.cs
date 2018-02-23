using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models
{
    public class SpendData
    {
        public decimal GasSpendAmount { get; set; }
        public UsagePeriod GasSpendPeriod { get; set; }
        public decimal ElectricitySpendAmount { get; set; }
        public UsagePeriod ElectricitySpendPeriod { get; set; }
    }
}