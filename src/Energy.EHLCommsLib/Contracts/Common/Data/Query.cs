using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.Common.Data
{
    [DataContract]
    public class Query
    {
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Name = "parameters")]
        public List<Item> Parameters { get; set; }
    }
}
