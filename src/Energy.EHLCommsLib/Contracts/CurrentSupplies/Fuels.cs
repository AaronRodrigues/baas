using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.CurrentSupplies
{
    [DataContract]
    public class Fuels
    {
        public Fuels()
        {
            Gas = new Fuel();
            Electricity = new Fuel();
        }

        [DataMember(Name = "gas")]
        public Fuel Gas { get; set; }

        [DataMember(Name = "electricity")]
        public Fuel Electricity { get; set; }
    }
}
