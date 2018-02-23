using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.CurrentSupply;
using Energy.EHLCommsLib.Contracts.SwitchApiResponse;

namespace Energy.EHLCommsLib.Contracts.Responses
{
    [DataContract]
    public class SwitchApiResponse : ApiResponse
    {
        public SwitchApiResponse()
        {
            Links = new List<Link>();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(Name = "currentSupply", EmitDefaultValue = false)]
        public EnergySupply CurrentSupply { get; set; }

        [DataMember(Name = "supplyLocation", EmitDefaultValue = false)]
        public SupplyLocation SupplyLocation { get; set; }

        [DataMember(Name = "currentUsage", EmitDefaultValue = false)]
        public Usage.Usage Usage { get; set; }

        [DataMember(Name = "proRata", EmitDefaultValue = false)]
        public ProRata ProRata { get; set; }

        [DataMember(Name = "switchType", EmitDefaultValue = false)]
        public NameIdPair SwitchType { get; set; }

        [DataMember(Name = "resultPreferences", EmitDefaultValue = false)]
        public ResultPreferences ResultPreferences { get; set; }

        [DataMember(Name = "futureEnergySupply", EmitDefaultValue = false)]
        public EnergySupply FutureEnergySupply { get; set; }

        [DataMember(Name = "customerData", EmitDefaultValue = false)]
        public CustomerData CustomerData { get; set; }
    }
}