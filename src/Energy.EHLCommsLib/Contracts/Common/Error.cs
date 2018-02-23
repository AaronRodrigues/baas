using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.Common
{
    [DataContract]
    public class Error
    {
        [DataMember(Name = "group", EmitDefaultValue = false)]
        public string Group { get; set; }

        [DataMember(Name = "item", EmitDefaultValue = false)]
        public string Item { get; set; }

        [DataMember(Name = "message")]
        public Message Message { get; set; }
    }
}