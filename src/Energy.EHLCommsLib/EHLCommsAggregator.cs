using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.External.Exceptions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EhlCommsAggregator
    {
        private readonly ISwitchServiceHelper _switchServiceHelper;

        public EhlCommsAggregator(ISwitchServiceHelper switchServiceHelper)
        {
            _switchServiceHelper = switchServiceHelper;
        }

        //TO DO : Add this value to config AppSettings.Feature.TariffCustomFeatureEnabled or get from MVC
        public GetPricesResponse GetPrices(GetPricesRequest request, Dictionary<string, string> customFeatures,
            bool tariffCustomFeatureEnabled = false)
        {
            var response = new GetPricesResponse();
            var ehlApiCalls = new EhlApiCalls(_switchServiceHelper, request);
            var pricesRetrievedSuccess = false;

            try
            {
                //Log.Info(string.Format("GetPrices started for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
                var supplyStageResult = ehlApiCalls.GetSupplierEhlApiResponse(request, response);
                if (supplyStageResult.ApiCallWasSuccessful)
                {
                    var usageStageResult = ehlApiCalls.GetUsageEhlApiResponse(request, response, supplyStageResult.NextUrl);
                    if (usageStageResult.ApiCallWasSuccessful)
                    {
                        var proRataCalculationApplied = ehlApiCalls.UpdateCurrentSwitchStatus(request.SwitchUrl, response, request.IgnoreProRataComparison);
                        var preferencesStageresult = ehlApiCalls.GetPreferenceEhlApiResponse(request, response, usageStageResult.NextUrl);
                        if (preferencesStageresult.ApiCallWasSuccessful)
                        {
                            ehlApiCalls.PopulatePricesResponseWithFutureSuppliesFromEhl(request, response, customFeatures, preferencesStageresult.NextUrl, tariffCustomFeatureEnabled, proRataCalculationApplied);
                        }
                        else
                        {
                            response.ErrorStage = preferencesStageresult.ApiStage;
                            response.ErrorString = preferencesStageresult.ConcatenatedErrorString;
                        }
                    }
                    else
                    {
                        response.ErrorStage = usageStageResult.ApiStage;
                        response.ErrorString = usageStageResult.ConcatenatedErrorString;
                    }
                }
                else
                {
                    response.ErrorStage = supplyStageResult.ApiStage;
                    response.ErrorString = supplyStageResult.ConcatenatedErrorString;
                }
                pricesRetrievedSuccess = true;
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
    }
}
