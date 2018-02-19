using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.CurrentSupplies
{
    [DataContract]
    public class Fuel
    {
        public Fuel()
        {
            Suppliers = new List<Supplier>();
        }

        [DataMember(Name = "defaultSupplier")]
        public NameIdPair DefaultSupplier { get; set; }

        [DataMember(Name = "suppliers")]
        public List<Supplier> Suppliers { get; set; }
    }
}
