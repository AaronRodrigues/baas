using Energy.EHLCommsLib.Enums;

namespace Energy.ProviderAdapter.Models
{
    public class BillDetails {
        public int ElectricityUsage { get; set; }
        public UsagePeriod ElectricityUsagePeriod { get; set; }
        public int ElectricitySpend { get; set; }
        public UsagePeriod ElectricitySpendPeriod { get; set; }
        public int GasUsage { get; set; }
        public UsagePeriod GasUsagePeriod { get; set; }
        public int GasSpend { get; set; }
        public UsagePeriod GasSpendPeriod { get; set; }
    }
}