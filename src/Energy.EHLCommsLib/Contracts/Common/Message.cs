using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.Common
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "verbatim", EmitDefaultValue = false)]
        public bool Verbatim { get; set; }
    }
}