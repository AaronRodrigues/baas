using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models.Prices
{
    public class PriceResult
    {
        public PriceResult()
        {
            KeyFeatures = new List<PriceFeature>();
        }

        public string Id { get; set; }
        public string ResultId { get; set; }

        [XmlIgnore]
        public int SupplierId { get; set; }

        public string SupplierName { get; set; }
        public int SupplierRating { get; set; }

        [XmlIgnore]
        public string SupplierCss { get; set; }

        [XmlIgnore]
        public int TariffId { get; set; }

        public string TariffName { get; set; }

        [XmlIgnore]
        public string TariffDetailsUrl { get; set; }

        public bool HasTariffCustomFeature { get; set; }
        public string CustomFeatureText { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }

        [XmlIgnore]
        public int PaymentMethodId { get; set; }

        public bool CanApply { get; set; }
        public decimal AnnualSavings { get; set; }
        public decimal AnnualSpend { get; set; }
        public decimal? ElectricityAnnualSavings { get; set; }
        public decimal? ElectricityAnnualSpend { get; set; }
        public decimal? GasAnnualSavings { get; set; }
        public decimal? GasAnnualSpend { get; set; }
        public List<PriceFeature> KeyFeatures { get; set; }

        public List<PriceFeature> TopNFeatures
        {
            get
            {
                var selectedFeatures = new List<PriceFeature>();

                var cappedFeature = KeyFeatures.FirstOrDefault(k => k.Category == PriceFeatureCategory.CappedOrFixed);
                if (cappedFeature != null)
                    selectedFeatures.Add(cappedFeature);

                var specialOffersFeature =
                    KeyFeatures.FirstOrDefault(
                        k =>
                            k.Category == PriceFeatureCategory.SpecialOffers &&
                            k.Description.ToLower().Contains("cashback"));
                if (specialOffersFeature != null)
                {
                    selectedFeatures.Add(specialOffersFeature);
                    Cashback = true;
                }
                else
                {
                    Cashback = false;
                }

                return selectedFeatures;
            }
        }

        public decimal RenewableFuelPercentage { get; set; }
        // Price attributes
        public bool PaperLessBilling { get; set; }
        public bool PaperBilling { get; set; }
        public bool NoStandingCharges { get; set; }
        public bool CappedOrFixed { get; set; }
        public bool Green { get; set; }
        public bool AccurateMonthlyBilling { get; set; }
        public bool StayWarm { get; set; }
        public bool Economy10 { get; set; }
        public bool Cashback { get; set; }
        public bool Hide { get; set; }
        public decimal? TotalExitFees { get; set; }
        public decimal? ElectricityEstimatedMonthlyCost { get; set; }
        public decimal? GasEstimatedMonthlyCost { get; set; }
        public bool IsMostPopularTariffForUser { get; set; }
        public bool IsCollectiveTariff { get; set; }
        public bool IsExclusiveTariff { get; set; }
        public bool ShowInRelevancyBox { get; set; }
        public bool ShowStrapline { get; set; }
        public bool CheapestBigSupplier { get; set; }
        public bool CheapestLongFixed { get; set; }
        public bool Cheapest { get; set; }
        public bool CheapestMediumFixed { get; set; }
        public bool CheapestCanApply { get; set; }
        public bool CheapestGreen { get; set; }
        public bool CheapestHighestRated { get; set; }
        public bool CheapestShortFixed { get; set; }
    }
}