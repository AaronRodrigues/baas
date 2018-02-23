using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts
{
    [DataContract]
    public class TagTextPair
    {
        [DataMember(Name = "tags", EmitDefaultValue = false)]
        public string Tags { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }
    }
}