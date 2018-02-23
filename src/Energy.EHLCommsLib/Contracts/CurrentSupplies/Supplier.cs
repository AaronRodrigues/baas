using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.CurrentSupplies
{
    [DataContract]
    public class Supplier
    {
        public Supplier()
        {
            SupplierTariffs = new List<SupplierTariff>();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "defaultSupplierTariff")]
        public NameIdPair DefaultSupplierTariff { get; set; }

        [DataMember(Name = "supplierTariffs")]
        public List<SupplierTariff> SupplierTariffs { get; set; }
    }
}