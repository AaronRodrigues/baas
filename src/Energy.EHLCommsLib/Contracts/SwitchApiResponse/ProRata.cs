using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.SwitchApiResponse
{
    [DataContract]
    public class ProRata
    {
        [DataMember(Name = "proRataCalculationApplied")]
        public bool ProRataCalculationApplied { get; set; }

        [DataMember(Name = "expiresIn")]
        public string ExpiresIn { get; set; }

        [DataMember(Name = "hasCancellationFee", EmitDefaultValue = false)]
        public bool HasCancellationFee { get; set; }

        [DataMember(Name = "cancellationFeeMessage", EmitDefaultValue = false)]
        public string CancellationFeeMessage { get; set; }
    }
}