using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models
{
    public class BaseRequest
    {
        public string Postcode { get; set; }
        public string SwitchId { get; set; }
        public EnergyJourneyType EnergyJourneyType { get; set; }
    }
}