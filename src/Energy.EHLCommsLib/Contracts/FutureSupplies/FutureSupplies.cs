using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.CurrentSupply;
using Energy.EHLCommsLib.Contracts.Responses;

namespace Energy.EHLCommsLib.Contracts.FutureSupplies
{
    [DataContract]
    public class FutureSupplies : ApiResponse
    {
        [DataMember(Name = "paymentMethods")]
        public List<NameIdPair> PaymentMethods { get; set; }

        [DataMember(Name = "attributes")]
        public List<string> Attributes { get; set; }

        [DataMember(Name = "results")]
        public List<FutureSupplyResultSet> Results { get; set; }

        [DataMember(Name = "supplyTypes")]
        public List<NameIdPair> SupplyTypes { get; set; }

        [DataMember(Name = "currentSupply")]
        public EnergySupply CurrentSupply { get; set; }

        [DataMember(Name = "currentUsage")]
        public Usage.Usage Usage { get; set; }
    }
}