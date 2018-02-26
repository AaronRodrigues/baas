using System.Collections.Generic;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class ClientPriceResultsMarketingFeedRequest
    {
        public string CompareWhat { get; set; }
        public string Postcode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ElectricityBillDate { get; set; }
        public string ElectricityBillPeriod { get; set; }
        public string GasBillDate { get; set; }
        public string GasBillPeriod { get; set; }
        public string HouseSize { get; set; }
        public string NoOfOccupants { get; set; }
        public bool EmailOptIn { get; set; }
        public List<MarketingFeedPriceResult> PriceResults { get; set; }
    }
}
