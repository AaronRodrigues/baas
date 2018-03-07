using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EhlApiCalls
    {

        private readonly IEhlHttpClient _ehlHttpClient;
        private readonly string _journeyId;

        public EhlApiCalls(IEhlHttpClient ehlHttpClient, string journeyId)
        {
           _ehlHttpClient = ehlHttpClient;
            _journeyId = journeyId;
        }

        public EhlApiResponse GetSupplierEhlApiResponse(GetPricesRequest request, GetPricesResponse response)
        {
            var currentSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(request.CurrentSupplyUrl);
            if (!currentSwitchesApiResponse.SuccessfulResponseFromEhl())
                return ApiCallResponse("CustomerSupplyStage", response, currentSwitchesApiResponse, EhlApiConstants.UsageRel);
            request.PopulateCurrentSupplyWithRequestData(currentSwitchesApiResponse);
            var currentSwitchesApiPostResponse = _ehlHttpClient.PostApiGetResponse(request.CurrentSupplyUrl, currentSwitchesApiResponse);
            return ApiCallResponse("CustomerSupplyStage", response, currentSwitchesApiPostResponse, EhlApiConstants.UsageRel);
        }

        public EhlApiResponse GetUsageEhlApiResponse(GetPricesRequest request, GetPricesResponse response, string url)
        {
            var usageSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(url);
            request.PopulateUsageWithRequestData(usageSwitchesApiResponse);
            var usageSwitchesApiPostResponse = _ehlHttpClient.PostApiGetResponse(url, usageSwitchesApiResponse);
            return ApiCallResponse("UsageStage", response, usageSwitchesApiPostResponse, EhlApiConstants.PreferenceRel);
        }

        public EhlApiResponse GetPreferenceEhlApiResponse(GetPricesRequest request, GetPricesResponse response, string url)
        {
            var preferencesSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(url);
            request.PopulatePreferencesWithRequestData(preferencesSwitchesApiResponse);
            var preferencesSwitchesApiPostResponse = _ehlHttpClient.PostApiGetResponse(url, preferencesSwitchesApiResponse);
            return ApiCallResponse("PreferencesStage", response, preferencesSwitchesApiPostResponse, EhlApiConstants.FutureSupplyRel);
        }

        public bool UpdateCurrentSwitchStatus(GetPricesRequest request, GetPricesResponse response)
        {
            var switchesUrl = request.SwitchUrl;
            var ignoreProRataComparison = request.IgnoreProRataComparison;
            var proRataCalculationApplied = false;
            var switchStatus = _ehlHttpClient.GetApiResponse<SwitchApiResponse>(switchesUrl);
            if (switchStatus == null) return proRataCalculationApplied;
            response.CurrentSupplyDetailsUrl = switchStatus.CurrentSupply.Details.Uri;
            if (switchStatus.Links.Count <= 0) return proRataCalculationApplied;
            var proRataUrl = switchStatus.Links.SingleOrDefault(l => l.Rel.Equals(EhlApiConstants.ProRataRel));
            if (string.IsNullOrWhiteSpace(proRataUrl?.Uri)) return proRataCalculationApplied;
            var proRataTemplate = _ehlHttpClient.GetApiResponse<ApiResponse>(proRataUrl.Uri);
            var proRataValue = ignoreProRataComparison ? "false" : "true";
            proRataTemplate.UpdateItemData("proRataPreference", "preferProRataCalculations",
                proRataValue);
            _ehlHttpClient.PostApiGetResponse(proRataUrl.Uri, proRataTemplate);
            proRataCalculationApplied = !ignoreProRataComparison;
            return proRataCalculationApplied;
        }

        public void PopulatePricesResponseWithFutureSuppliesFromEhl(GetPricesRequest request, GetPricesResponse response, Dictionary<string, string> customFeatures, 
            string futureSupplyUrl, bool tariffCustomFeatureEnabled, bool proRataCalculationApplied)
        {
            var futureSupplySwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(futureSupplyUrl);
            var futureSuppliesUrl = futureSupplySwitchesApiResponse.GetLinkedDataUrl(EhlApiConstants.FutureSuppliesRel);
            var futureSuppliesApiPostResponse = _ehlHttpClient.GetApiResponse<FutureSupplies>(futureSuppliesUrl);
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
