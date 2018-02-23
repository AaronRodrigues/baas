using System.Collections.Generic;

namespace Energy.EHLCommsLib.Models.Prices
{
    public class GetPricesResponse : BaseResponse
    {
        public string CurrentSupplyDetailsUrl { get; set; }
        public string FutureSupplyUrl { get; set; }
        public string QuoteUrl { get; set; }
        public decimal AnnualEstimatedBill { get; set; }
        public UsageData EstimatedUsage { get; set; }
        public List<PriceResult> Results { get; set; }
        public bool ProRataCalculationApplied { get; set; }
        public string ErrorStage { get; set; }
        public string ErrorString { get; set; }
    }
}