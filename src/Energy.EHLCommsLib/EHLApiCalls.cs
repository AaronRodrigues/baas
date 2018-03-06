using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.Http;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EhlApiCalls
    {

        private readonly IEhlHttpClient _ehlHttpClient;
        private string _journeyId;

        public EhlApiCalls(string journeyId)
        {
           _ehlHttpClient = new EhlHttpClient();
            _journeyId = journeyId;
        }

        public EhlApiResponse GetSupplierEhlApiResponse(GetPricesRequest request, GetPricesResponse response)
        {
            var currentSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(request.CurrentSupplyUrl, _journeyId);
            if (!currentSwitchesApiResponse.SuccessfulResponseFromEhl())
                return ApiCallResponse("CustomerSupplyStage", response, currentSwitchesApiResponse, EhlApiConstants.UsageRel);
            request.PopulateCurrentSupplyWithRequestData(currentSwitchesApiResponse);
            var currentSwitchesApiPostResponse = _ehlHttpClient.PostSwitchesApiGetResponse(request.CurrentSupplyUrl, currentSwitchesApiResponse, _journeyId);
            return ApiCallResponse("CustomerSupplyStage", response, currentSwitchesApiPostResponse, EhlApiConstants.UsageRel);
        }

        public EhlApiResponse GetUsageEhlApiResponse(GetPricesRequest request, GetPricesResponse response, string url)
        {
            var usageSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(url, _journeyId);
            request.PopulateUsageWithRequestData(usageSwitchesApiResponse);
            var usageSwitchesApiPostResponse = _ehlHttpClient.PostSwitchesApiGetResponse(url, usageSwitchesApiResponse, _journeyId);
            return ApiCallResponse("UsageStage", response, usageSwitchesApiPostResponse, EhlApiConstants.PreferenceRel);
        }

        public EhlApiResponse GetPreferenceEhlApiResponse(GetPricesRequest request, GetPricesResponse response, string url)
        {
            var preferencesSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(url, _journeyId);
            request.PopulatePreferencesWithRequestData(preferencesSwitchesApiResponse);
            var preferencesSwitchesApiPostResponse = _ehlHttpClient.PostSwitchesApiGetResponse(url, preferencesSwitchesApiResponse, _journeyId);
            return ApiCallResponse("PreferencesStage", response, preferencesSwitchesApiPostResponse, EhlApiConstants.FutureSupplyRel);
        }

        public bool UpdateCurrentSwitchStatus(GetPricesRequest request, GetPricesResponse response)
        {
            var switchesUrl = request.SwitchUrl;
            var ignoreProRataComparison = request.IgnoreProRataComparison;
            var proRataCalculationApplied = false;
            var switchStatus = _ehlHttpClient.GetApiResponse<SwitchApiResponse>(switchesUrl, _journeyId);
            if (switchStatus == null) return proRataCalculationApplied;
            response.CurrentSupplyDetailsUrl = switchStatus.CurrentSupply.Details.Uri;
            if (switchStatus.Links.Count <= 0) return proRataCalculationApplied;
            var proRataUrl = switchStatus.Links.SingleOrDefault(l => l.Rel.Equals(EhlApiConstants.ProRataRel));
            if (string.IsNullOrWhiteSpace(proRataUrl?.Uri)) return proRataCalculationApplied;
            var proRataTemplate = _ehlHttpClient.GetApiResponse<ApiResponse>(proRataUrl.Uri, _journeyId);
            var proRataValue = ignoreProRataComparison ? "false" : "true";
            proRataTemplate.UpdateItemData("proRataPreference", "preferProRataCalculations",
                proRataValue);
            _ehlHttpClient.PostSwitchesApiGetResponse(proRataUrl.Uri, proRataTemplate, _journeyId);
            proRataCalculationApplied = !ignoreProRataComparison;
            return proRataCalculationApplied;
        }

        public void PopulatePricesResponseWithFutureSuppliesFromEhl(GetPricesRequest request, GetPricesResponse response, Dictionary<string, string> customFeatures, 
            string futureSupplyUrl, bool tariffCustomFeatureEnabled, bool proRataCalculationApplied)
        {
            var futureSupplySwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(futureSupplyUrl, _journeyId);
            var futureSuppliesUrl = futureSupplySwitchesApiResponse.GetLinkedDataUrl(EhlApiConstants.FutureSuppliesRel);
            var futureSuppliesApiPostResponse = _ehlHttpClient.GetApiResponse<FutureSupplies>(futureSuppliesUrl,_journeyId);
            response.PopulatePricesResponse(request, futureSuppliesApiPostResponse, customFeatures, futureSupplyUrl, tariffCustomFeatureEnabled, proRataCalculationApplied);
        }

        private EhlApiResponse ApiCallResponse(string typeOfRequest, GetPricesResponse response, ApiResponse switchesApiResponse, string rel = "")
        {
            if (!switchesApiResponse.SuccessfulResponseFromEhl())
            {
                response.HydrateSwitchResponseWithErrors(switchesApiResponse.Errors);
            }
            if (switchesApiResponse.Errors == null || !switchesApiResponse.Errors.Any())
                return new EhlApiResponse
                {
                    ApiCallWasSuccessful = true,
                    ApiStage = typeOfRequest,
                    ConcatenatedErrorString = string.Empty,
                    NextUrl = rel == string.Empty ? "" : switchesApiResponse.GetNextRelUrl(rel)
                };
            var errString = string.Empty;
            errString = switchesApiResponse.Errors.Aggregate(errString, (current, error) => current + error.Message.Text);
            return new EhlApiResponse { ApiCallWasSuccessful = false, ApiStage = typeOfRequest, ConcatenatedErrorString = errString };
        }
    }
}
