using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Contracts.CurrentSupplies;

namespace Energy.EHLCommsLib.Contracts.FutureSupplies
{
    [DataContract]
    public class FutureSupplyResult
    {
        [DataMember(Name = "expectedAnnualSpend")]
        public decimal ExpectedAnnualSpend { get; set; }

        [DataMember(Name = "supplier")]
        public Supplier Supplier { get; set; }

        [DataMember(Name = "supplyDetails")]
        public SupplyDetails SupplyDetails { get; set; }

        [DataMember(Name = "expectedAnnualSavings")]
        public decimal ExpectedAnnualSavings { get; set; }

        [DataMember(Name = "expectedGasAnnualSpend", EmitDefaultValue = false)]
        public decimal? ExpectedGasAnnualSpend { get; set; }

        [DataMember(Name = "expectedGasAnnualSavings", EmitDefaultValue = false)]
        public decimal? ExpectedGasAnnualSavings { get; set; }

        [DataMember(Name = "expectedElecAnnualSpend", EmitDefaultValue = false)]
        public decimal? ExpectedElecAnnualSpend { get; set; }

        [DataMember(Name = "expectedElecAnnualSavings", EmitDefaultValue = false)]
        public decimal? ExpectedElecAnnualSavings { get; set; }

        [DataMember(Name = "estimatedGasMonthlyCost", EmitDefaultValue = false)]
        public decimal? EstimatedGasMonthlyCost { get; set; }

        [DataMember(Name = "estimatedElecMonthlyCost", EmitDefaultValue = false)]
        public decimal? EstimatedElecMonthlyCost { get; set; }

        [DataMember(Name = "canApply")]
        public bool CanApply { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "promotions")]
        public List<string> Promotions { get; set; }
    }
}
