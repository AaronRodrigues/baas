using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Contracts.CurrentSupplies
{
    [DataContract]
    public class FuelPlan
    {
        [DataMember(Name = "fuelType", Order = 1)]
        public string FuelType { get; set; }

        [DataMember(Name = "eco7", Order = 2)]
        public bool Eco7 { get; set; }

        [DataMember(Name = "nonEco7", Order = 3)]
        public bool NonEco7 { get; set; }
    }
}
