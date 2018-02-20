using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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
