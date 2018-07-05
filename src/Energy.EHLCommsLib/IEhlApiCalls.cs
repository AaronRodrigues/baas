using System.Collections.Generic;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public interface IEhlApiCalls
    {
        Task<EhlApiResponse> GetSupplierEhlApiResponse(GetPricesRequest request, string environment);
        Task<EhlApiResponse> GetUsageEhlApiResponse(GetPricesRequest request, string url, string environment);
        Task<EhlApiResponse> GetPreferenceEhlApiResponse(GetPricesRequest request, string url, string environment);
        Task UpdateCurrentSwitchStatus(GetPricesRequest request, string environment);
        Task<List<PriceResult>> PopulatePricesResponseWithFutureSuppliesFromEhl(GetPricesRequest request,  
            string futureSupplyUrl,  string environment);
    }
}