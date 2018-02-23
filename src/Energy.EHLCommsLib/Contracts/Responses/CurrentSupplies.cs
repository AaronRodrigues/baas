using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.CurrentSupplies;

namespace Energy.EHLCommsLib.Contracts.Responses
{
    [DataContract]
    public class CurrentSupplies : ApiResponse
    {
        public CurrentSupplies()
        {
            Fuels = new Fuels();
        }

        [DataMember(Name = "fuels")]
        public Fuels Fuels { get; set; }

        [DataMember(Name = "paymentMethods")]
        public List<NameIdPair> PaymentMethods { get; set; }

        [DataMember(Name = "attributes")]
        public List<string> Attributes { get; set; }

        [DataMember(Name = "specialBehaviourAttributes")]
        public List<string> SpecialBehaviourAttributes { get; set; }
    }
}