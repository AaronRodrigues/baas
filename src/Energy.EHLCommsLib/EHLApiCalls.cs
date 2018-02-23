using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EhlApiCalls
    {

        private readonly ISwitchServiceHelper _switchServiceHelper;
        private BaseRequest _baseRequest;

        public EhlApiCalls(ISwitchServiceHelper switchServiceHelper, BaseRequest baseRequest)
        {
            _switchServiceHelper = switchServiceHelper;
            _baseRequest = baseRequest;
        }

        public EhlApiResponse GetSupplierEhlApiResponse(GetPricesRequest request, GetPricesResponse response)
        {
            var currentSwitchesApiResponse = _switchServiceHelper.GetApiDataTemplate(request.CurrentSupplyUrl, EhlApiConstants.CurrentSupplyRel);
            if (!_switchServiceHelper.SuccessfulResponseFromEhl(currentSwitchesApiResponse))
                return ApiCallResponse("CustomerSupplyStage", response, currentSwitchesApiResponse, EhlApiConstants.UsageRel);
            request.PopulateCurrentSupplyWithRequestData(currentSwitchesApiResponse);
            var currentSwitchesApiPostResponse = _switchServiceHelper.GetSwitchesApiPostResponse(request.CurrentSupplyUrl, currentSwitchesApiResponse, EhlApiConstants.CurrentSupplyRel, _baseRequest);
            return ApiCallResponse("CustomerSupplyStage", response, currentSwitchesApiPostResponse, EhlApiConstants.UsageRel);
        }

        public EhlApiResponse GetUsageEhlApiResponse(GetPricesRequest request, GetPricesResponse response, string url)
        {
            var usageSwitchesApiResponse = _switchServiceHelper.GetApiDataTemplate(url, EhlApiConstants.UsageRel);
            request.PopulateUsageWithRequestData(usageSwitchesApiResponse);
            var usageSwitchesApiPostResponse = _switchServiceHelper.GetSwitchesApiPostResponse(url, usageSwitchesApiResponse, EhlApiConstants.UsageRel, _baseRequest);
            return ApiCallResponse("UsageStage", response, usageSwitchesApiPostResponse, EhlApiConstants.PreferenceRel);
        }

        public EhlApiResponse GetPreferenceEhlApiResponse(GetPricesRequest request, GetPricesResponse response, string url)
        {
            var preferencesSwitchesApiResponse = _switchServiceHelper.GetApiDataTemplate(url, EhlApiConstants.PreferenceRel);
            request.PopulatePreferencesWithRequestData(preferencesSwitchesApiResponse);
            var preferencesSwitchesApiPostResponse = _switchServiceHelper.GetSwitchesApiPostResponse(url, preferencesSwitchesApiResponse, EhlApiConstants.PreferenceRel, _baseRequest);
            return ApiCallResponse("PreferencesStage", response, preferencesSwitchesApiPostResponse, EhlApiConstants.FutureSupplyRel);
        }

        public bool UpdateCurrentSwitchStatus(string switchesUrl, GetPricesResponse response, bool ignoreProRataComparison)
        {
            var proRataCalculationApplied = false;
            var switchStatus = _switchServiceHelper.GetSwitchesApiGetResponse<SwitchApiResponse>(switchesUrl, EhlApiConstants.SwitchRel, _baseRequest);

            if (switchStatus != null)
            {
                response.CurrentSupplyDetailsUrl = switchStatus.CurrentSupply.Details.Uri;
                if (switchStatus.Links.Count > 0)
                {
                    var proRataUrl = switchStatus.Links.SingleOrDefault(l => l.Rel.Equals(EhlApiConstants.ProRataRel));
                    if (proRataUrl != null && !string.IsNullOrWhiteSpace(proRataUrl.Uri))
                    {
                        var proRataTemplate = _switchServiceHelper.GetApiDataTemplate(proRataUrl.Uri, EhlApiConstants.ProRataRel);
                        var proRataValue = ignoreProRataComparison ? "false" : "true";
                        _switchServiceHelper.UpdateItemData(proRataTemplate, "proRataPreference", "preferProRataCalculations",
                            proRataValue);
                        _switchServiceHelper.GetSwitchesApiPostResponse(proRataUrl.Uri, proRataTemplate, EhlApiConstants.ProRataRel, _baseRequest);
                        proRataCalculationApplied = !ignoreProRataComparison;
                    }
                }
            }
            return proRataCalculationApplied;
        }

        public void PopulatePricesResponseWithFutureSuppliesFromEhl(GetPricesRequest request, GetPricesResponse response, Dictionary<string, string> customFeatures, 
            string futureSupplyUrl, bool tariffCustomFeatureEnabled, bool proRataCalculationApplied)
        {
            var futureSupplySwitchesApiResponse = _switchServiceHelper.GetApiDataTemplate(futureSupplyUrl, EhlApiConstants.FutureSupplyRel);
            var futureSuppliesUrl = _switchServiceHelper.GetLinkedDataUrl(futureSupplySwitchesApiResponse, EhlApiConstants.FutureSuppliesRel);
            var futureSuppliesApiPostResponse = _switchServiceHelper.GetSwitchesApiGetResponse<FutureSupplies>(futureSuppliesUrl,
                    EhlApiConstants.FutureSuppliesRel, _baseRequest);
            response.PopulatePricesResponse(request, futureSuppliesApiPostResponse, customFeatures, futureSupplyUrl, tariffCustomFeatureEnabled, proRataCalculationApplied);
        }

        private EhlApiResponse ApiCallResponse(string typeOfRequest, GetPricesResponse response, SwitchesApiResponse switchesApiResponse, string rel = "")
        {
            if (!_switchServiceHelper.SuccessfulResponseFromEhl(switchesApiResponse))
            {
                _switchServiceHelper.HydrateSwitchResponseWithErrors(response, switchesApiResponse.Errors);
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
