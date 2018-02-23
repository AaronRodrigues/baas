using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Mappers;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Extensions
{
    public static class GetPricesResponseExtensions
    {
        public static GetPricesResponse PopulatePricesResponse(this GetPricesResponse getPricesResponse,
            GetPricesRequest request, FutureSupplies futureSuppliesResponse, Dictionary<string, string> customFeatures,
            string futureSupplyUrl, bool tariffCustomFeatureEnabled, bool proRataCalculationApplied)
        {
            var quoteLinkUrl = futureSuppliesResponse.Links.First(l => l.Rel.Contains(EhlApiConstants.QuoteLinkRel)).Uri;
            getPricesResponse.FutureSupplyUrl = futureSupplyUrl;
            getPricesResponse.AnnualEstimatedBill = GetCurrentAnnualSpend(futureSuppliesResponse, request);
            getPricesResponse.EstimatedUsage = GetEstimatedUsage(futureSuppliesResponse);
            getPricesResponse.Results = futureSuppliesResponse.MapToPriceResults(request, customFeatures,
                tariffCustomFeatureEnabled);
            getPricesResponse.ProRataCalculationApplied = proRataCalculationApplied;
            getPricesResponse.QuoteUrl = quoteLinkUrl;
            return getPricesResponse;
        }

        private static decimal GetCurrentAnnualSpend(FutureSupplies futureSuppliesResponse, GetPricesRequest request)
        {
            var currentBillValue = 0M;

            if (request.CompareType != CompareWhat.Electricity)
                currentBillValue += futureSuppliesResponse.Usage.Gas.AnnualSpend;

            if (request.CompareType != CompareWhat.Gas)
                currentBillValue += futureSuppliesResponse.Usage.Elec.AnnualSpend;

            return decimal.Round(currentBillValue, 0);
        }

        private static UsageData GetEstimatedUsage(FutureSupplies futureSuppliesResponse)
        {
            var estimatedUsage = new UsageData();

            if (futureSuppliesResponse.CurrentSupply.Electricity != null)
                estimatedUsage.ElectricityKwh = futureSuppliesResponse.Usage.Elec.AnnualKWh;

            if (futureSuppliesResponse.CurrentSupply.Gas != null)
                estimatedUsage.GasKwh = futureSuppliesResponse.Usage.Gas.AnnualKWh;

            return estimatedUsage;
        }
    }
}