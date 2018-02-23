using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common;

namespace Energy.EHLCommsLib.Contracts.CurrentSupply
{
    [DataContract]
    public class EnergySupply
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string SupplyId { get; set; }

        [DataMember(Name = "supplyType")]
        public string SupplyType { get; set; }

        [DataMember(Name = "gas", EmitDefaultValue = false)]
        public Gas Gas { get; set; }

        [DataMember(Name = "electricity", EmitDefaultValue = false)]
        public Electricity Electricity { get; set; }

        [DataMember(Name = "details", EmitDefaultValue = false)]
        public Link Details { get; set; }
    }
}