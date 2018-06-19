using System;
using System.Collections.Generic;
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

        public List<PriceResult> GetPrices(GetPricesRequest request, string environment)
        {
            try
            {
                return ApplyDataFromEhlToPricesResponse(request, environment);
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
        
        private List<PriceResult> ApplyDataFromEhlToPricesResponse(GetPricesRequest request, string environment)
        {
            //Log.Info(string.Format("GetPrices started for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
            if (SupplyStageUrl(request, environment, out var supplyStageResult)) return null;
            if (UsageStageResult(request, environment, supplyStageResult, out var usageStageResult)) return null;

            var proRataCalculationApplied = _ehlApiCalls.UpdateCurrentSwitchStatus(request, environment);
            var preferencesStageresult = _ehlApiCalls.GetPreferenceEhlApiResponse(request, usageStageResult.NextUrl, environment);
            
            return _ehlApiCalls.PopulatePricesResponseWithFutureSuppliesFromEhl(request,
                preferencesStageresult.NextUrl, proRataCalculationApplied, environment);

        }

        private bool UsageStageResult(GetPricesRequest request, string environment, EhlApiResponse supplyStageResult,
            out EhlApiResponse usageStageResult)
        {
            usageStageResult = _ehlApiCalls.GetUsageEhlApiResponse(request, supplyStageResult.NextUrl, environment);
            if (!usageStageResult.ApiCallWasSuccessful)
            {
                //response.ErrorStage = usageStageResult.ApiStage;
                //response.ErrorString = usageStageResult.ConcatenatedErrorString;
                return true;
            }

            return false;
        }

        private bool SupplyStageUrl(GetPricesRequest request, string environment, out EhlApiResponse supplyStageResult)
        {
            supplyStageResult = _ehlApiCalls.GetSupplierEhlApiResponse(request, environment);
            if (!supplyStageResult.ApiCallWasSuccessful)
            {
                //response.ErrorStage = supplyStageResult.ApiStage;
                //response.ErrorString = supplyStageResult.ConcatenatedErrorString;
                return true;
            }

            return false;
        }
    }
}
