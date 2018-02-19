using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Contracts.Common.Data
{
    [DataContract]
    public class DataTemplate
    {
        [DataMember(Name = "groups")]
        public List<Group> Groups { get; set; }

        [DataMember(Name = "methods")]
        public List<string> Methods { get; set; }

        [DataMember(Name = "validateAs", EmitDefaultValue = false)]
        public string ValidateAs { get; set; }
    }
}
