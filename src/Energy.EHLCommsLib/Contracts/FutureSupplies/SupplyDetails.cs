using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.FutureSupplies
{
    [DataContract]
    public class SupplyDetails
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "gasTariff", EmitDefaultValue = false)]
        public NameIdPair GasTariff { get; set; }

        [DataMember(Name = "elecTariff", EmitDefaultValue = false)]
        public NameIdPair ElecTariff { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "logo")]
        public Link Logo { get; set; }

        [DataMember(Name = "details")]
        public Link FurtherDetails { get; set; }

        [DataMember(Name = "renewableFuelPercentage")]
        public decimal RenewableFuelPercentage { get; set; }

        [DataMember(Name = "paymentMethod")]
        public NameIdPair PaymentMethod { get; set; }

        [DataMember(Name = "keyFeatures")]
        public List<TagTextPair> KeyFeatures { get; set; }

        [DataMember(Name = "attributes")]
        public List<string> Attributes { get; set; }

        [DataMember(Name = "elecCancellationFee")]
        public decimal ExitFeesElectricity { get; set; }

        [DataMember(Name = "gasCancellationFee")]
        public decimal ExitFeesGas { get; set; }
    }
}