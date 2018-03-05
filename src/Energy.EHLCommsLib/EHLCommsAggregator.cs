using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.External.Exceptions;
using Energy.EHLCommsLib.External.Services;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    //TO DO : Logging
    //TO DO : Add this value to config AppSettings.Feature.TariffCustomFeatureEnabled or get from MVC
    public class EhlCommsAggregator
    {
        private readonly ISwitchServiceHelper _switchServiceHelper;

        public EhlCommsAggregator(ISwitchServiceClient switchServiceClient)
        {
            _switchServiceHelper = new SwitchServiceHelper(switchServiceClient);
        }
        public GetPricesResponse GetPrices(GetPricesRequest request, Dictionary<string, string> customFeatures,
            bool tariffCustomFeatureEnabled = false)
        {
            var response = new GetPricesResponse();

            try
            {
                var pricesRetrievedSuccess = ApplyDataFromEhlToPricesResponse(request, response, customFeatures, tariffCustomFeatureEnabled);
            }
            catch (InvalidSwitchException)
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

            return response;
        }

        private bool ApplyDataFromEhlToPricesResponse(GetPricesRequest request, GetPricesResponse response, Dictionary<string, string> customFeatures,
            bool tariffCustomFeatureEnabled)
        {
            var ehlApiCalls = new EhlApiCalls(_switchServiceHelper, request);
            //Log.Info(string.Format("GetPrices started for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
            var supplyStageResult = ehlApiCalls.GetSupplierEhlApiResponse(request, response);
            if (!supplyStageResult.ApiCallWasSuccessful)
            {
                response.ErrorStage = supplyStageResult.ApiStage;
                response.ErrorString = supplyStageResult.ConcatenatedErrorString;
                return false;
            }
            var usageStageResult = ehlApiCalls.GetUsageEhlApiResponse(request, response, supplyStageResult.NextUrl);
            if (!usageStageResult.ApiCallWasSuccessful)
            {
                response.ErrorStage = usageStageResult.ApiStage;
                response.ErrorString = usageStageResult.ConcatenatedErrorString;
                return false;
            }
            var proRataCalculationApplied = ehlApiCalls.UpdateCurrentSwitchStatus(request, response);
            var preferencesStageresult = ehlApiCalls.GetPreferenceEhlApiResponse(request, response, usageStageResult.NextUrl);
            if (!preferencesStageresult.ApiCallWasSuccessful)
            {
                response.ErrorStage = preferencesStageresult.ApiStage;
                response.ErrorString = preferencesStageresult.ConcatenatedErrorString;
            }
            ehlApiCalls.PopulatePricesResponseWithFutureSuppliesFromEhl(request, response, customFeatures, preferencesStageresult.NextUrl, tariffCustomFeatureEnabled, proRataCalculationApplied);
            return true;
        }
    }
}
