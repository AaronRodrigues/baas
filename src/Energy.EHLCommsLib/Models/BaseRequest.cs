using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models
{
    public abstract class BaseRequest
    {
        public string Postcode { get; set; }
        public string SwitchId { get; set; }
        public EnergyJourneyType EnergyJourneyType { get; set; }
    }
}