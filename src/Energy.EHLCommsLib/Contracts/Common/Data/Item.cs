using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.Common.Data
{
    [DataContract]
    [KnownType(typeof (List<string>))]
    [KnownType(typeof (ArrayList))]
    public class Item
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "data")]
        public object Data { get; set; }

        [DataMember(Name = "prompt", EmitDefaultValue = false)]
        public string Prompt { get; set; }

        [DataMember(Name = "mandatory", EmitDefaultValue = false)]
        public bool Mandatory { get; set; }

        [DataMember(Name = "validateAs", EmitDefaultValue = false)]
        public string ValidateAs { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(Name = "rel", EmitDefaultValue = false)]
        public string Rel { get; set; }

        [DataMember(Name = "acceptableValues", EmitDefaultValue = false)]
        public List<NameIdPair> AcceptableValues { get; set; }

        [DataMember(Name = "regularExpression", EmitDefaultValue = false)]
        public string RegularExpression { get; set; }

        [DataMember(Name = "regularExpressionErrorMessage", EmitDefaultValue = false)]
        public string RegularExpressionErrorMessage { get; set; }

        [DataMember(Name = "validationFunction", EmitDefaultValue = false)]
        public string ValidationFunction { get; set; }

        [DataMember(Name = "validationDocument", EmitDefaultValue = false)]
        public string ValidationDocument { get; set; }

        [DataMember(Name = "readOnly", EmitDefaultValue = false)]
        public bool ReadOnly { get; set; }

        [DataMember(Name = "verbatim", EmitDefaultValue = false)]
        public bool Verbatim { get; set; }

        [DataMember(Name = "statements", EmitDefaultValue = false)]
        public List<Statement> Statements { get; set; }

        [DataMember(Name = "guidance", EmitDefaultValue = false)]
        public List<Statement> Guidance { get; set; }
    }
}