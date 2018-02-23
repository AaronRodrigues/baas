using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.Common.Data
{
    [DataContract]
    public class NameIdPair
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }
    }
}