using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Contracts
{
    [DataContract]
    public class Environmental
    {
        [DataMember(Name = "fuelMix", EmitDefaultValue = false)]
        public SupplierFuelMix SupplierFuelMix { get; set; }

        [DataMember(Name = "CO2EmissionPerKwh", EmitDefaultValue = false)]
        public decimal? Co2Emission { get; set; }

        [DataMember(Name = "NuclearWastePerKwh", EmitDefaultValue = false)]
        public decimal? NuclearWaste { get; set; }
    }
}
