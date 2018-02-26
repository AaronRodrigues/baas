using System.Runtime.Serialization;

//Signup package
namespace Energy.EHLCommsLib.Contracts.Responses
{
    [DataContract]
    public class AddressSearchResult
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "knownGasMeter")]
        public bool KnownGasMeter { get; set; }

        [DataMember(Name = "knownElectricityMeter")]
        public bool KnownElectricityMeter { get; set; }

        [DataMember(Name = "igtWarning")]
        public string IGTWarning { get; set; }
    }
}