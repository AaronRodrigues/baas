using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.CurrentSupply
{
    [DataContract]
    public class Electricity
    {
        [DataMember(Name = "supplier")]
        public NameIdPair Supplier { get; set; }

        [DataMember(Name = "supplierTariff")]
        public NameIdPair SupplierTariff { get; set; }

        [DataMember(Name = "paymentMethod")]
        public NameIdPair PaymentMethod { get; set; }

        [DataMember(Name = "economy7")]
        public bool Economy7 { get; set; }
    }
}