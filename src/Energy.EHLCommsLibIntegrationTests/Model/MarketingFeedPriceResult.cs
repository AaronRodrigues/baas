using System;
using System.Xml.Serialization;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class MarketingFeedPriceResult
    {
        public string SupplierName { get; set; }
        public string SupplierLogoUrl { get; set; }
        public string SupplierRating { get; set; }
        public string TariffName { get; set; }
        public string PaymentMethod { get; set; }
        public string AnnualSaving { get; set; }
        public string AnnualSpend { get; set; }

        [XmlElement(IsNullable = true)]
        public string ElectricityAnnualSavings { get; set; }

        [XmlElement(IsNullable = true)]
        public string ElectricityAnnualSpend { get; set; }

        [XmlElement(IsNullable = true)]
        public string GasAnnualSavings { get; set; }

        [XmlElement(IsNullable = true)]
        public string GasAnnualSpend { get; set; }

        public string DetailsUrl
        {
            get
            {
                return "https://my.dev.internal.comparethemarket.com/" + "?energyretrieve";
            }

            // empty setter allows property to be serialized
            set { throw new NotImplementedException(); }
        }
    }
}
