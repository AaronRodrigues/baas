using Energy.EHLCommsLib.Enums;

namespace Energy.ProviderAdapter.Models
{
    public class UsageData
    {
        public int GasKwh { get; set; }
        public UsagePeriod GasUsagePeriod { get; set; }
        public int ElectricityKwh { get; set; }
        public UsagePeriod ElectricityUsagePeriod { get; set; }
    }
}
