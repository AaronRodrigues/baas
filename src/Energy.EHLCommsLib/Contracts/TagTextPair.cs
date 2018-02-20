using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
