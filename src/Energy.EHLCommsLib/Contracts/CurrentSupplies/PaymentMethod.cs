using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.CurrentSupplies
{
    [DataContract]
    public class PaymentMethod
    {
        public PaymentMethod()
        {
            For = new List<FuelPlan>();
        }

        [DataMember(Name = "id", Order = 1)]
        public string Id { get; set; }

        [DataMember(Name = "for", Order = 2)]
        public List<FuelPlan> For { get; set; }
    }
}