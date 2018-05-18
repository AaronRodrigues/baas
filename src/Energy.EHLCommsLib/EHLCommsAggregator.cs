using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.Exceptions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    //TO DO : Logging
    //TO DO : Add this value to config AppSettings.Feature.TariffCustomFeatureEnabled or get from MVC
    public class EhlCommsAggregator
    {
        private readonly IEhlHttpClient _ehlHttpClient;

        public EhlCommsAggregator(IEhlHttpClient ehlHttpClient)
        {
            _ehlHttpClient = ehlHttpClient;
        }
        public List<PriceResult> GetPrices(GetPricesRequest request)
        {
            try
            {
                return ApplyDataFromEhlToPricesResponse(request);
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
        
        private List<PriceResult> ApplyDataFromEhlToPricesResponse(GetPricesRequest request)
        {
            var ehlApiCalls = new EhlApiCalls(_ehlHttpClient, request.JourneyId.ToString());
            //Log.Info(string.Format("GetPrices started for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
            var supplyStageResult = ehlApiCalls.GetSupplierEhlApiResponse(request);
            if (!supplyStageResult.ApiCallWasSuccessful)
            {
                //response.ErrorStage = supplyStageResult.ApiStage;
                //response.ErrorString = supplyStageResult.ConcatenatedErrorString;
                return null;
            }
            var usageStageResult = ehlApiCalls.GetUsageEhlApiResponse(request, supplyStageResult.NextUrl);
            if (!usageStageResult.ApiCallWasSuccessful)
            {
                //response.ErrorStage = usageStageResult.ApiStage;
                //response.ErrorString = usageStageResult.ConcatenatedErrorString;
                return null;
            }
            var proRataCalculationApplied = ehlApiCalls.UpdateCurrentSwitchStatus(request);
            var preferencesStageresult = ehlApiCalls.GetPreferenceEhlApiResponse(request, usageStageResult.NextUrl);
            if (preferencesStageresult.ApiCallWasSuccessful)
                return ehlApiCalls.PopulatePricesResponseWithFutureSuppliesFromEhl(request,
                    preferencesStageresult.NextUrl, proRataCalculationApplied);
            //response.ErrorStage = preferencesStageresult.ApiStage;
            //response.ErrorString = preferencesStageresult.ConcatenatedErrorString;
            return ehlApiCalls.PopulatePricesResponseWithFutureSuppliesFromEhl(request,
                preferencesStageresult.NextUrl, proRataCalculationApplied);

        }
    }
}
