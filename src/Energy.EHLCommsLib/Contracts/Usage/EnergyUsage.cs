using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Contracts.Usage
{
    [DataContract]
    public class EnergyUsage
    {
        [DataMember(Name = "usageProfile")]
        public Origin Origin { get; set; }

        [DataMember(Name = "annualKWh")]
        public int AnnualKWh { get; set; }

        [DataMember(Name = "annualSpend")]
        public decimal AnnualSpend { get; set; }
    }
}
