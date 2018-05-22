using Energy.EHLCommsLib.Enums;

namespace Energy.ProviderAdapter
{
    public class NoBillDetails {
        public int ElectricitySpend { get; set; }
        public UsagePeriod ElectricitySpendPeriod { get; set; }
        public int GasSpend { get; set; }
        public UsagePeriod GasSpendPeriod { get; set; }
        public bool ElectricitySpendUnknown { get; set; }
        public bool GasSpendUnknown { get; set; }
        public int NumberOfBedrooms { get; set; }
        public int NumberOfOccupants { get; set; }
        public string MainHeatingSource { get; set; }
        public string HeatingUsage { get; set; }
        public string HouseInsulation { get; set; }
        public string MainCookingSource { get; set; }
        public bool HouseOccupied { get; set; }
        public string HouseType { get; set; }
    }
}