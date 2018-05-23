using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Enums;

namespace Energy.ProviderAdapter.Models
{
    public class EnergyRisk
    {
        public BillDetails Bill { get; set; }
        public NoBillDetails NoBill { get; set; }
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string JourneyId { get; set; }
        public string Postcode { get; set; }
        public string SwitchId { get; set; }
        public EnergyJourneyType EnergyJourneyType { get; set; }
        public string CurrentSupplyUrl { get; set; }
        public string SwitchUrl { get; set; }
        public string PrePayment { get; set; }
        public CompareWhat CompareType { get; set; }
        public int GasSupplierId { get; set; }
        public int GasTariffId { get; set; }
        public int GasPaymentMethodId { get; set; }
        public int ElectricitySupplierId { get; set; }
        public int ElectricityTariffId { get; set; }
        public int ElectricityPaymentMethodId { get; set; }
        public bool Economy7 { get; set; }
        public int Economy7NightUsage { get; set; }
        public bool IgnoreProRataComparison { get; set; }
    }
}
