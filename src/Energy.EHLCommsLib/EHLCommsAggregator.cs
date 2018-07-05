using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EhlCommsAggregator : IEhlCommsAggregator
    {
        private readonly IEhlApiCalls _ehlApiCalls;

        public EhlCommsAggregator(IEhlApiCalls ehlApiCalls) => _ehlApiCalls = ehlApiCalls;

        public async Task<List<PriceResult>> GetPrices(GetPricesRequest request, string environment)
            => await ApplyDataFromEhlToPricesResponse(request, environment).ConfigureAwait(false);

        private async Task<List<PriceResult>> ApplyDataFromEhlToPricesResponse(GetPricesRequest request, string environment)
        {
            var usageStageUrl = await UsageStageResult(request, environment).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(usageStageUrl))
            {
                throw new NullReferenceException($"Supply Stage/ Usage Stage url is not available for SwitchUrl : {request.SwitchUrl}, SwitchId : {request.SwitchId}");
            }

            await _ehlApiCalls.UpdateCurrentSwitchStatus(request, environment).ConfigureAwait(false);
            var preferencesStageresult = await _ehlApiCalls.GetPreferenceEhlApiResponse(request, usageStageUrl, environment).ConfigureAwait(false);
            return await _ehlApiCalls.PopulatePricesResponseWithFutureSuppliesFromEhl(request,
                preferencesStageresult.NextUrl, environment).ConfigureAwait(false);
        }

        private async Task<string> UsageStageResult(GetPricesRequest request, string environment)
        {
            var supplyStageUrl = await SupplyStageUrl(request, environment).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(supplyStageUrl))
            {
                return string.Empty;
            }

            var usageStageResult = await _ehlApiCalls.GetUsageEhlApiResponse(request, supplyStageUrl, environment).ConfigureAwait(false);
            return usageStageResult.NextUrl;
        }

        private async Task<string> SupplyStageUrl(GetPricesRequest request, string environment)
        {
            var supplyStageResult = await _ehlApiCalls.GetSupplierEhlApiResponse(request, environment).ConfigureAwait(false);
            return supplyStageResult.NextUrl;
        }
    }
}