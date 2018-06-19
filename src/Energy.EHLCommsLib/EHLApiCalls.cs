using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Mappers;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    //TO DO : Process Ehl Errors

    public class EhlApiCalls : IEhlApiCalls
    {
        private readonly IEhlHttpClient _ehlHttpClient;

        public EhlApiCalls(IEhlHttpClient ehlHttpClient)
        {
           _ehlHttpClient = ehlHttpClient;
        }

        public EhlApiResponse GetSupplierEhlApiResponse(GetPricesRequest request, string environment)
        {
            var currentSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(request.CurrentSupplyUrl, environment);
            if (!currentSwitchesApiResponse.SuccessfulResponseFromEhl())
                return ApiCallResponse("CustomerSupplyStage", currentSwitchesApiResponse, EhlApiConstants.UsageRel);

            request.PopulateCurrentSupplyWithRequestData(currentSwitchesApiResponse);
            var currentSwitchesApiPostResponse = _ehlHttpClient.PostApiGetResponse(request.CurrentSupplyUrl, currentSwitchesApiResponse, environment);
            return ApiCallResponse("CustomerSupplyStage", currentSwitchesApiPostResponse, EhlApiConstants.UsageRel);
        }

        public EhlApiResponse GetUsageEhlApiResponse(GetPricesRequest request, string url, string environment)
        {
            var usageSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(url, environment);
            request.PopulateUsageWithRequestData(usageSwitchesApiResponse);
            var usageSwitchesApiPostResponse = _ehlHttpClient.PostApiGetResponse(url, usageSwitchesApiResponse, environment);
            return ApiCallResponse("UsageStage", usageSwitchesApiPostResponse, EhlApiConstants.PreferenceRel);
        }

        public EhlApiResponse GetPreferenceEhlApiResponse(GetPricesRequest request, string url, string environment)
        {
            var preferencesSwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(url, environment);
            request.PopulatePreferencesWithRequestData(preferencesSwitchesApiResponse);
            var preferencesSwitchesApiPostResponse = _ehlHttpClient.PostApiGetResponse(url, preferencesSwitchesApiResponse, environment);
            return ApiCallResponse("PreferencesStage", preferencesSwitchesApiPostResponse, EhlApiConstants.FutureSupplyRel);
        }

        public bool UpdateCurrentSwitchStatus(GetPricesRequest request, string environment)
        {
            var switchesUrl = request.SwitchUrl;
            var ignoreProRataComparison = request.IgnoreProRataComparison;
            var proRataCalculationApplied = false;
            var switchStatus = _ehlHttpClient.GetApiResponse<SwitchApiResponse>(switchesUrl, environment);
            if (switchStatus == null) return proRataCalculationApplied;
            if (switchStatus.Links.Count <= 0) return proRataCalculationApplied;
            var proRataUrl = switchStatus.Links.SingleOrDefault(l => l.Rel.Equals(EhlApiConstants.ProRataRel));
            if (string.IsNullOrWhiteSpace(proRataUrl?.Uri)) return proRataCalculationApplied;
            var proRataTemplate = _ehlHttpClient.GetApiResponse<ApiResponse>(proRataUrl.Uri, environment);
            var proRataValue = ignoreProRataComparison ? "false" : "true";
            proRataTemplate.UpdateItemData("proRataPreference", "preferProRataCalculations",
                proRataValue);
            _ehlHttpClient.PostApiGetResponse(proRataUrl.Uri, proRataTemplate, environment);
            proRataCalculationApplied = !ignoreProRataComparison;
            return proRataCalculationApplied;
        }

        public List<PriceResult> PopulatePricesResponseWithFutureSuppliesFromEhl(GetPricesRequest request,  
            string futureSupplyUrl, bool proRataCalculationApplied, string environment)
        {
            var futureSupplySwitchesApiResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(futureSupplyUrl, environment);
            var futureSuppliesUrl = futureSupplySwitchesApiResponse.GetLinkedDataUrl(EhlApiConstants.FutureSuppliesRel);
            var futureSuppliesApiPostResponse = _ehlHttpClient.GetApiResponse<FutureSupplies>(futureSuppliesUrl, environment);
            return futureSuppliesApiPostResponse.MapToPriceResults(request);
        }

        //To DO : Process Ehl errors ??
        private EhlApiResponse ApiCallResponse(string typeOfRequest, ApiResponse switchesApiResponse, string rel = "")
        {
            if (!switchesApiResponse.SuccessfulResponseFromEhl())
            {
               //response.HydrateSwitchResponseWithErrors(switchesApiResponse.Errors);
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
