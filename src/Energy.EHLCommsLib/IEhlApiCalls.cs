using System.Collections.Generic;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public interface IEhlApiCalls
    {
        EhlApiResponse GetSupplierEhlApiResponse(GetPricesRequest request, string environment);
        EhlApiResponse GetUsageEhlApiResponse(GetPricesRequest request, string url, string environment);
        EhlApiResponse GetPreferenceEhlApiResponse(GetPricesRequest request, string url, string environment);
        bool UpdateCurrentSwitchStatus(GetPricesRequest request, string environment);

        List<PriceResult> PopulatePricesResponseWithFutureSuppliesFromEhl(GetPricesRequest request,  
            string futureSupplyUrl, bool proRataCalculationApplied, string environment);
    }
}