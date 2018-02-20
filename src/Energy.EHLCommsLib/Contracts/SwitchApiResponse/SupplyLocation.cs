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
    public class SupplyLocation
    {
        [DataMember(Name = "region")]
        public NameIdPair Region { get; set; }

        [DataMember(Name = "gasExitZone")]
        public string GasExitZone { get; set; }

        [DataMember(Name = "supplyPostcode")]
        public string SupplyPostcode { get; set; }

    }
}
