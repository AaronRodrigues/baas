using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Contracts.CurrentSupplies
{
    [DataContract]
    public class SupplierTariff
    {
        public SupplierTariff()
        {
            PaymentMethods = new List<PaymentMethod>();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "paymentMethods")]
        public List<PaymentMethod> PaymentMethods { get; set; }

        [DataMember(Name = "attributes", EmitDefaultValue = false)]
        public List<string> Attributes { get; set; }
    }
}
