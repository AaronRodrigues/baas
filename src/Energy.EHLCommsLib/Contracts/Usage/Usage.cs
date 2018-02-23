using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.SwitchApiResponse;

namespace Energy.EHLCommsLib.Contracts.Usage
{
    [DataContract]
    public class Usage
    {
        [DataMember(Name = "gas", EmitDefaultValue = false)]
        public EnergyUsage Gas { get; set; }

        [DataMember(Name = "elec", EmitDefaultValue = false)]
        public EnergyUsage Elec { get; set; }

        [DataMember(Name = "seasonalAdjustment", EmitDefaultValue = false)]
        public SeasonalAdjustment SeasonalAdjustment { get; set; }
    }
}