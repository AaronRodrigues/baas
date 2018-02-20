using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts
{
    [DataContract]
    public class Group
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "items", EmitDefaultValue = false)]
        public List<Item> Items { get; set; }

        [DataMember(Name = "validateAs", EmitDefaultValue = false)]
        public string ValidateAs { get; set; }

        [DataMember(Name = "mandatory", EmitDefaultValue = false)]
        public bool Mandatory { get; set; }

        [DataMember(Name = "title", EmitDefaultValue = false)]
        public string Title { get; set; }

        [DataMember(Name = "verbatim", EmitDefaultValue = false)]
        public bool Verbatim { get; set; }

        [DataMember(Name = "guidance", EmitDefaultValue = false)]
        public List<Statement> Guidance { get; set; }

        [DataMember(Name = "statements", EmitDefaultValue = false)]
        public List<Statement> Statements { get; set; }

        [DataMember(Name = "remoteValidation", EmitDefaultValue = false)]
        public string RemoteValidationRel { get; set; }

        [DataMember(Name = "tags", EmitDefaultValue = false)]
        public string Tags { get; set; }
    }
}
