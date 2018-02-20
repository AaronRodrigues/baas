using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Contracts.Common;

namespace Energy.EHLCommsLib.Contracts
{
    [DataContract]
    public class Supplier
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "serviceStarRating", EmitDefaultValue = false)]
        public int ServiceStarRating { get; set; }

        [DataMember(Name = "logo", EmitDefaultValue = false)]
        public Link Logo { get; set; }

        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Name = "environmental", EmitDefaultValue = false)]
        public Environmental Environmental { get; set; }
    }
}
