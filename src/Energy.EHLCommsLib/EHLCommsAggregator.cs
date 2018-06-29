using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Exceptions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    //TO DO : Logging
    //TO DO : Add this value to config AppSettings.Feature.TariffCustomFeatureEnabled or get from MVC
    public class EhlCommsAggregator : IEhlCommsAggregator
    {
        private readonly IEhlApiCalls _ehlApiCalls;

        public EhlCommsAggregator(IEhlApiCalls ehlApiCalls)
        {
            _ehlApiCalls = ehlApiCalls;
       }

        public async Task<List<PriceResult>> GetPrices(GetPricesRequest request, string environment)
        {
            try
            {
                return await ApplyDataFromEhlToPricesResponse(request, environment).ConfigureAwait(false);
            }
            catch (InvalidSwitchException ex)
            {
                //Log.Info(string.Format("Invalid switch for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
                throw;
            }
            catch (Exception ex)
            {
                //Log.Error(string.Format("Exception occurred while calling EHL API. JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}, Exception = {3}", request.JourneyId, request.SwitchId, request.SwitchUrl, ex));
                throw;
            }
            finally
            {
                //Log.Info(!pricesRetrievedSuccess
                //    ? string.Format("GetPrices failed for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl)
                //    : string.Format("GetPrices finished successfully for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
            }
        }
        
        private async Task<List<PriceResult>> ApplyDataFromEhlToPricesResponse(GetPricesRequest request, string environment)
        {
            //Log.Info(string.Format("GetPrices started for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
            var usageStageUrl = await UsageStageResult(request, environment).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(usageStageUrl))
            {
                return null;
            }

            var proRataCalculationApplied = await _ehlApiCalls.UpdateCurrentSwitchStatus(request, environment).ConfigureAwait(false);
            var preferencesStageresult = await _ehlApiCalls.GetPreferenceEhlApiResponse(request, usageStageUrl, environment).ConfigureAwait(false);
            
            return await _ehlApiCalls.PopulatePricesResponseWithFutureSuppliesFromEhl(request,
                preferencesStageresult.NextUrl, proRataCalculationApplied, environment).ConfigureAwait(false);
        }

        private async Task<string> UsageStageResult(GetPricesRequest request, string environment)
        {
            var supplyStageUrl = await SupplyStageUrl(request, environment).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(supplyStageUrl))
            {
                return string.Empty;
            }

            var usageStageResult = await _ehlApiCalls.GetUsageEhlApiResponse(request, supplyStageUrl, environment).ConfigureAwait(false);
            return usageStageResult.ApiCallWasSuccessful ? usageStageResult.NextUrl : string.Empty;
        }

        private async Task<string> SupplyStageUrl(GetPricesRequest request, string environment)
        {
            var supplyStageResult = await _ehlApiCalls.GetSupplierEhlApiResponse(request, environment).ConfigureAwait(false);
            return supplyStageResult.ApiCallWasSuccessful ? supplyStageResult.NextUrl : string.Empty;
        }
    }
}
