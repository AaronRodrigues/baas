using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts
{
    [DataContract]
    public class SupplierFuelMix
    {
        [DataMember(Name = "coal")]
        public decimal Coal { get; set; }

        [DataMember(Name = "naturalGas", EmitDefaultValue = false)]
        public decimal NaturalGas { get; set; }

        [DataMember(Name = "nuclear", EmitDefaultValue = false)]
        public decimal Nuclear { get; set; }

        [DataMember(Name = "renewable", EmitDefaultValue = false)]
        public decimal Renewable { get; set; }

        [DataMember(Name = "other", EmitDefaultValue = false)]
        public decimal Other { get; set; }
    }
}
