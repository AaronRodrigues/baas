using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.FutureSupplies
{
    [DataContract]
    public class FutureSupplyResultSet
    {
        [DataMember(Name = "supplyType")]
        public NameIdPair SupplyType { get; set; }

        [DataMember(Name = "resultCount")]
        public int ResultCount { get; set; }

        [DataMember(Name = "energySupplies")]
        public List<FutureSupplyResult> EnergySupplies { get; set; }
    }
}
