using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.SwitchApiResponse
{
    [DataContract]
    public class SeasonalAdjustment
    {
        [DataMember(Name = "lastBillingPeriod")]
        public NameIdPair LastBillingPeriod { get; set; }

        [DataMember(Name = "fuelUsedForHeating")]
        public string FuelUsedForHeating { get; set; }
    }
}