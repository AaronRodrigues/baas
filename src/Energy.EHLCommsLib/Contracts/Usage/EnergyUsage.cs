using System.Runtime.Serialization;

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