using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.Common.Data
{
    [DataContract]
    public class Statement
    {
        [DataMember(Name = "reference", IsRequired = true)]
        public string Reference { get; set; }

        [DataMember(Name = "title", EmitDefaultValue = false)]
        public string Title { get; set; }

        [DataMember(Name = "statement", EmitDefaultValue = false)]
        public string StatementText { get; set; }

        [DataMember(Name = "verbatim", EmitDefaultValue = false)]
        public bool Verbatim { get; set; }

        [DataMember(Name = "appliesWhenMatches", EmitDefaultValue = false)]
        public string AppliesWhenMatches { get; set; }

        [DataMember(Name = "link", EmitDefaultValue = false)]
        public Link Link { get; set; }

        [DataMember(Name = "isFragment", EmitDefaultValue = false)]
        public bool IsFragment { get; set; }
    }
}
