using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.Usage
{
    [DataContract]
    public class Origin
    {
        [DataMember(Name = "usageType")]
        public NameIdPair Type { get; set; }

        [DataMember(Name = "usage")]
        public Group UsageProfile { get; set; }
    }
}