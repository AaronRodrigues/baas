using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
    public class EhlApiCalls : IEhlApiCalls
    {
        private readonly IEhlHttpClient _ehlHttpClient;

        public EhlApiCalls(IEhlHttpClient ehlHttpClient)
        {
           _ehlHttpClient = ehlHttpClient;
        }

        public async Task<EhlApiResponse> GetSupplierEhlApiResponse(GetPricesRequest request, string environment)
        {
            var currentSwitchesApiResponse = await _ehlHttpClient.GetApiResponse<ApiResponse>(request.CurrentSupplyUrl, environment).ConfigureAwait(false);
            CheckSuccessReponse("CustomerSupplyStage", currentSwitchesApiResponse);

            request.PopulateCurrentSupplyWithRequestData(currentSwitchesApiResponse);
            var currentSwitchesApiPostResponse = await _ehlHttpClient.PostApiGetResponse(request.CurrentSupplyUrl, currentSwitchesApiResponse, environment).ConfigureAwait(false);
            return ApiCallResponse("CustomerSupplyStage", currentSwitchesApiPostResponse, EhlApiConstants.UsageRel);
        }

        public async Task<EhlApiResponse> GetUsageEhlApiResponse(GetPricesRequest request, string url, string environment)
        {
            var usageSwitchesApiResponse = await _ehlHttpClient.GetApiResponse<ApiResponse>(url, environment).ConfigureAwait(false);
            CheckSuccessReponse("UsageStage", usageSwitchesApiResponse);

            request.PopulateUsageWithRequestData(usageSwitchesApiResponse);
            var usageSwitchesApiPostResponse = await _ehlHttpClient.PostApiGetResponse(url, usageSwitchesApiResponse, environment).ConfigureAwait(false);
            return ApiCallResponse("UsageStage", usageSwitchesApiPostResponse, EhlApiConstants.PreferenceRel);
        }

        public async Task<EhlApiResponse> GetPreferenceEhlApiResponse(GetPricesRequest request, string url, string environment)
        {
            var preferencesSwitchesApiResponse = await _ehlHttpClient.GetApiResponse<ApiResponse>(url, environment).ConfigureAwait(false);
            CheckSuccessReponse("PreferencesStage", preferencesSwitchesApiResponse);

            request.PopulatePreferencesWithRequestData(preferencesSwitchesApiResponse);
            var preferencesSwitchesApiPostResponse = await _ehlHttpClient.PostApiGetResponse(url, preferencesSwitchesApiResponse, environment).ConfigureAwait(false);
            return ApiCallResponse("PreferencesStage", preferencesSwitchesApiPostResponse, EhlApiConstants.FutureSupplyRel);
        }

        public async Task UpdateCurrentSwitchStatus(GetPricesRequest request, string environment)
        {
            var switchesUrl = request.SwitchUrl;
            var ignoreProRataComparison = request.IgnoreProRataComparison;
            var switchStatus = await _ehlHttpClient.GetApiResponse<SwitchApiResponse>(switchesUrl, environment).ConfigureAwait(false);
            if (switchStatus?.Links == null)
            {
                return;
            }
            var proRataUrl = switchStatus.Links.SingleOrDefault(l => l.Rel.Equals(EhlApiConstants.ProRataRel));
            if (string.IsNullOrWhiteSpace(proRataUrl?.Uri))
            {
                return;
            }
            var proRataTemplate = await _ehlHttpClient.GetApiResponse<ApiResponse>(proRataUrl.Uri, environment).ConfigureAwait(false);
            var proRataValue = ignoreProRataComparison ? "false" : "true";
            proRataTemplate.UpdateItemData("proRataPreference", "preferProRataCalculations", proRataValue);
            await _ehlHttpClient.PostApiGetResponse(proRataUrl.Uri, proRataTemplate, environment).ConfigureAwait(false);
        }

        public async Task<List<PriceResult>> PopulatePricesResponseWithFutureSuppliesFromEhl(GetPricesRequest request,  
            string futureSupplyUrl, string environment)
        {
            var futureSupplySwitchesApiResponse = await _ehlHttpClient.GetApiResponse<ApiResponse>(futureSupplyUrl, environment).ConfigureAwait(false);
            CheckSuccessReponse("FutureSupplySwitches", futureSupplySwitchesApiResponse);
            var futureSuppliesUrl = futureSupplySwitchesApiResponse.GetLinkedDataUrl(EhlApiConstants.FutureSuppliesRel);
            var futureSuppliesGetResponse = await _ehlHttpClient.GetApiResponse<FutureSupplies>(futureSuppliesUrl, environment).ConfigureAwait(false);
            CheckSuccessReponse("FutureSuppliesSwitches", futureSuppliesGetResponse);
            return futureSuppliesGetResponse.MapToPriceResults(request);
        }

        private EhlApiResponse ApiCallResponse(string typeOfRequest, ApiResponse apiResponse, string rel)
        {
            CheckSuccessReponse(typeOfRequest, apiResponse);

            return new EhlApiResponse
            {
                ApiCallWasSuccessful = true,
                NextUrl = apiResponse.GetNextRelUrl(rel)
            };
        }

        private static void CheckSuccessReponse(string typeOfRequest, ApiResponse apiResponse)
        {
            if (apiResponse == null)
            {
                throw new NullReferenceException($"{typeOfRequest} : Deserialization of API Response failed");
            }
            if (apiResponse.SuccessfulResponseFromEhl())
            {
                return;
            }
            var errString = string.Empty;
            errString = apiResponse.Errors.Aggregate(errString, (current, error) => current + error.Message.Text);
            throw new HttpRequestException($"{typeOfRequest} : {errString}");
        }
    }
}
