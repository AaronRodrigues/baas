
using System;
using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.External.Exceptions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Mappers;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EhlCommsAggregator
    {
        private const string UsageTypeByEstimator = "1";
        private const string UsageTypeByKWhUsage = "3";
        private const string UsageTypeBySpend = "4";

        private const string SwitchRel = "/rels/domestic/switch";
        private const string CurrentSupplyRel = "/rels/domestic/current-supply";
        private const string ProRataRel = "/rels/domestic/prorata-preference";
        private const string UsageRel = "/rels/domestic/usage";
        private const string PreferenceRel = "/rels/domestic/preferences";
        private const string FutureSupplyRel = "/rels/domestic/future-supply";
        private const string FutureSuppliesRel = "/rels/domestic/future-supplies";
        private const string QuoteLinkRel = "/rels/domestic/quote";

        private readonly ISwitchServiceHelper _switchServiceHelper;
        private BaseRequest _baseRequest;
        private SwitchesApiResponse _currentSupplyPostResponse;
        private SwitchesApiResponse _usagePostResponse;
        private SwitchesApiResponse _preferencesPostResponse;
        private bool _tariffCustomFeatureEnabled;

        private bool _proRataCalculationApplied;

        public EhlCommsAggregator(ISwitchServiceHelper switchServiceHelper, bool tariffCustomFeatureEnabled = false)
        {
            //AppSettings.Feature.TariffCustomFeatureEnabled
            _tariffCustomFeatureEnabled = tariffCustomFeatureEnabled;
            _switchServiceHelper = switchServiceHelper;
        }

        public GetPricesResponse GetPrices(GetPricesRequest request, Dictionary<string, string> customFeatures)
        {
            var response = new GetPricesResponse();
            _baseRequest = request;
            var pricesRetrievedSuccess = true;

            try
            {
                //Log.Info(string.Format("GetPrices started for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));

                var isSupplyStageSuccessful = CustomerSupplyStageIsSuccessful(request, response);
                if (isSupplyStageSuccessful.ApiCallWasSuccessful)
                {
                    var isUsageStageSuccessful = UsageStageIsSuccessful(request, response);
                    if (isUsageStageSuccessful.ApiCallWasSuccessful)
                    {
                        UpdateCurrentSwitchStatus(request.SwitchUrl, response, request.IgnoreProRataComparison);
                        var isPreferencesStageSuccessful = PreferencesStageIsSuccessful(request, response);
                        if (isPreferencesStageSuccessful.ApiCallWasSuccessful)
                        {
                            PopulatePricesResponse(request, response, customFeatures);
                        }
                        else
                        {
                            response.ErrorStage = isPreferencesStageSuccessful.ApiStage;
                            response.ErrorString = isPreferencesStageSuccessful.ConcatenatedErrorString;
                        }
                    }
                    else
                    {
                        response.ErrorStage = isUsageStageSuccessful.ApiStage;
                        response.ErrorString = isUsageStageSuccessful.ConcatenatedErrorString;
                    }
                }
                else
                {
                    response.ErrorStage = isSupplyStageSuccessful.ApiStage;
                    response.ErrorString = isSupplyStageSuccessful.ConcatenatedErrorString;
                }
            }
            catch (InvalidSwitchException)
            {
                //Log.Info(string.Format("Invalid switch for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
                pricesRetrievedSuccess = false;
                throw;
            }
            catch (Exception ex)
            {
                //Log.Error(string.Format("Exception occurred while calling EHL API. JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}, Exception = {3}", request.JourneyId, request.SwitchId, request.SwitchUrl, ex));
                pricesRetrievedSuccess = false;
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

        private EhlApiResponse ApiCallResponse(string typeOfRequest, bool hasWorked, IList<Error> errors = null)
        {
            var errString = "";
            if (errors != null && errors.Any())
            {
                errString = errors.Aggregate(errString, (current, error) => current + error.Message.Text);
                return new EhlApiResponse { ApiCallWasSuccessful = hasWorked, ApiStage = typeOfRequest, ConcatenatedErrorString = errString };
            }
            return new EhlApiResponse
            {
                ApiCallWasSuccessful = hasWorked,
                ApiStage = typeOfRequest,
                ConcatenatedErrorString = string.Empty
            };
        }


        private EhlApiResponse CustomerSupplyStageIsSuccessful(GetPricesRequest request, GetPricesResponse response)
        {
            _currentSupplyPostResponse = GetCurrentSupplyPostResponse(request);
            if (!_switchServiceHelper.SuccessfulResponseFromEhl(_currentSupplyPostResponse))
            {
                _switchServiceHelper.HydrateSwitchResponseWithErrors(response, _currentSupplyPostResponse.Errors);
                return ApiCallResponse("CustomerSupplyStage", false, _currentSupplyPostResponse.Errors);
            }
            return ApiCallResponse("CustomerSupplyStage", true);
        }

        private EhlApiResponse UsageStageIsSuccessful(GetPricesRequest request, GetPricesResponse response)
        {
            var usageUrl = _currentSupplyPostResponse.GetNextRelUrl(UsageRel);
            _usagePostResponse = GetUsagePostResponse(request, usageUrl);
            if (!_switchServiceHelper.SuccessfulResponseFromEhl(_usagePostResponse))
            {
                _switchServiceHelper.HydrateSwitchResponseWithErrors(response, _usagePostResponse.Errors);
                return ApiCallResponse("UsageStage", false, _usagePostResponse.Errors);
            }
            return ApiCallResponse("UsageStage", true);
        }

        private EhlApiResponse PreferencesStageIsSuccessful(GetPricesRequest request, GetPricesResponse response)
        {
            var preferencesUrl = _usagePostResponse.GetNextRelUrl(PreferenceRel);
            _preferencesPostResponse = GetPreferencesPostResponse(preferencesUrl, request);
            if (!_switchServiceHelper.SuccessfulResponseFromEhl(_preferencesPostResponse))
            {
                _switchServiceHelper.HydrateSwitchResponseWithErrors(response, _preferencesPostResponse.Errors);
                return ApiCallResponse("PreferencesStage", false, _preferencesPostResponse.Errors);
            }
            return ApiCallResponse("PreferencesStage", true);
        }

        private void PopulatePricesResponse(GetPricesRequest request, GetPricesResponse response, Dictionary<string, string> customFeatures)
        {
            var futureSupplyUrl = _preferencesPostResponse.GetNextRelUrl(FutureSupplyRel);
            var futureSupplyTemplate = _switchServiceHelper.GetApiDataTemplate(futureSupplyUrl, FutureSupplyRel);

            var futureSuppliesUrl = _switchServiceHelper.GetLinkedDataUrl(futureSupplyTemplate, FutureSuppliesRel);
            var futureSuppliesResponse = _switchServiceHelper.GetSwitchesApiGetResponse<FutureSupplies>(futureSuppliesUrl,
                    FutureSuppliesRel, _baseRequest);
            var quoteLinkUrl = futureSuppliesResponse.Links.First(l => l.Rel.Contains(QuoteLinkRel)).Uri;
            response.FutureSupplyUrl = futureSupplyUrl;
            response.AnnualEstimatedBill = GetCurrentAnnualSpend(futureSuppliesResponse, request);
            response.EstimatedUsage = GetEstimatedUsage(futureSuppliesResponse);
            response.Results = futureSuppliesResponse.MapToPriceResults(request, customFeatures,_tariffCustomFeatureEnabled);
            response.ProRataCalculationApplied = _proRataCalculationApplied;
            response.QuoteUrl = quoteLinkUrl;
        }

        private SwitchesApiResponse GetCurrentSupplyPostResponse(GetPricesRequest request)
        {
            var currentSupplyTemplate = _switchServiceHelper.GetApiDataTemplate(request.CurrentSupplyUrl, CurrentSupplyRel);
            if (_switchServiceHelper.SuccessfulResponseFromEhl(currentSupplyTemplate))
            {
                PopulateCurrentSupplyWithRequestData(request, currentSupplyTemplate);
                return _switchServiceHelper.GetSwitchesApiPostResponse(request.CurrentSupplyUrl, currentSupplyTemplate, CurrentSupplyRel, _baseRequest);
            }
            return currentSupplyTemplate;
        }

        private void PopulateCurrentSupplyWithRequestData(GetPricesRequest request, SwitchesApiResponse currentSupplyTemplate)
        {
            var compareGas = request.CompareType != CompareWhat.Electricity;
            var compareElec = request.CompareType != CompareWhat.Gas;

            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "includedFuels", "compareGas", compareGas.ToString());
            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "includedFuels", "compareElec", compareElec.ToString());

            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "gasTariff", "supplier", request.GasSupplierId.ToString());
            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "gasTariff", "supplierTariff", request.GasTariffId.ToString());
            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "gasTariff", "paymentMethod", request.GasPaymentMethodId.ToString());

            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "elecTariff", "supplier", request.ElectricitySupplierId.ToString());
            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "elecTariff", "supplierTariff", request.ElectricityTariffId.ToString());
            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "elecTariff", "paymentMethod", request.ElectricityPaymentMethodId.ToString());
            _switchServiceHelper.UpdateItemData(currentSupplyTemplate, "elecTariff", "economy7", request.ElectricityEco7.ToString());
        }

        private void UpdateCurrentSwitchStatus(string switchesUrl, GetPricesResponse response, bool ignoreProRataComparison)
        {
            var switchStatus = _switchServiceHelper.GetSwitchesApiGetResponse<SwitchApiResponse>(switchesUrl, SwitchRel, _baseRequest);

            if (switchStatus != null)
            {
                response.CurrentSupplyDetailsUrl = switchStatus.CurrentSupply.Details.Uri;

                ProcessProRataCalculations(switchStatus, ignoreProRataComparison);
            }
        }

        private void ProcessProRataCalculations(SwitchApiResponse switchStatus, bool ignoreProRataComparison)
        {
            if (switchStatus.Links.Count > 0)
            {
                var proRataUrl = switchStatus.Links.SingleOrDefault(l => l.Rel.Equals(ProRataRel));
                if (proRataUrl != null && !string.IsNullOrWhiteSpace(proRataUrl.Uri))
                {
                    ApplyProRataCalculations(proRataUrl.Uri, ignoreProRataComparison);
                    _proRataCalculationApplied = !ignoreProRataComparison;
                }
            }
        }

        private void ApplyProRataCalculations(string proRataUrl, bool ignoreProRataComparison)
        {
            var proRataTemplate = _switchServiceHelper.GetApiDataTemplate(proRataUrl, ProRataRel);
            var proRataValue = ignoreProRataComparison == true ? "false" : "true";
            _switchServiceHelper.UpdateItemData(proRataTemplate, "proRataPreference", "preferProRataCalculations",
                proRataValue);
            _switchServiceHelper.GetSwitchesApiPostResponse(proRataUrl, proRataTemplate, ProRataRel, _baseRequest);
        }

        private SwitchesApiResponse GetUsagePostResponse(GetPricesRequest request, string url)
        {
            var usageTemplate = _switchServiceHelper.GetApiDataTemplate(url, UsageRel);

            PopulateUsageWithRequestData(request, usageTemplate);

            return _switchServiceHelper.GetSwitchesApiPostResponse(url, usageTemplate, UsageRel, _baseRequest);
        }

        private void PopulateUsageWithRequestData(GetPricesRequest request, SwitchesApiResponse usageTemplate)
        {
            var compareGas = request.CompareType != CompareWhat.Electricity;
            var compareElec = request.CompareType != CompareWhat.Gas;

            _switchServiceHelper.UpdateItemData(usageTemplate, "includedFuels", "compareGas", compareGas.ToString());
            _switchServiceHelper.UpdateItemData(usageTemplate, "includedFuels", "compareElec", compareElec.ToString());

            switch (request.EnergyJourneyType)
            {
                case EnergyJourneyType.HaveMyBill:
                case EnergyJourneyType.Qr:
                    if (request.CalculateElecBasedOnBillSpend)
                    {
                        PopulateSpendUsageForElectricity(request, usageTemplate);
                    }
                    else
                    {
                        PopulateKWhUsage(request, usageTemplate);
                    }

                    if (request.CalculateGasBasedOnBillSpend)
                    {
                        PopulateSpendUsageForGas(request, usageTemplate);
                    }
                    else
                    {
                        PopulateKWhUsage(request, usageTemplate);
                    }

                    break;

                case EnergyJourneyType.DontHaveMyBill:
                    if (request.UseDetailedEstimatorForElectricity)
                        PopulateDetailedUsageForElectricity(request, usageTemplate);
                    else
                        PopulateSpendUsageForElectricity(request, usageTemplate);

                    if (request.UseDetailedEstimatorForGas)
                        PopulateDetailedUsageForGas(request, usageTemplate);
                    else
                        PopulateSpendUsageForGas(request, usageTemplate);

                    break;

            }

            if (compareElec && request.ElectricityEco7)
            {
                _switchServiceHelper.UpdateItemData(usageTemplate, "economy7", "nightUsagePercentage", request.PercentageNightUsage.ToString());
            }
        }

        private void PopulateKWhUsage(GetPricesRequest request, SwitchesApiResponse usageTemplate)
        {
            if (!request.CalculateGasBasedOnBillSpend)
            {
                _switchServiceHelper.UpdateItemData(usageTemplate, "gasUsageType", "usageType", UsageTypeByKWhUsage);
                _switchServiceHelper.UpdateItemData(usageTemplate, "gasKWhUsage", "usageAsKWh", request.UsageData.GasKwh.ToString());
                _switchServiceHelper.UpdateItemData(usageTemplate, "gasKWhUsage", "usagePeriod", ((int)request.UsageData.GasUsagePeriod).ToString());
            }

            if (!request.CalculateElecBasedOnBillSpend)
            {
                _switchServiceHelper.UpdateItemData(usageTemplate, "elecUsageType", "usageType", UsageTypeByKWhUsage);
                _switchServiceHelper.UpdateItemData(usageTemplate, "elecKWhUsage", "usageAsKWh", request.UsageData.ElectricityKwh.ToString());
                _switchServiceHelper.UpdateItemData(usageTemplate, "elecKWhUsage", "usagePeriod", ((int)request.UsageData.ElectricityUsagePeriod).ToString());
            }
        }

        private void PopulateSpendUsageForElectricity(GetPricesRequest request, SwitchesApiResponse usageTemplate)
        {
            var compareElec = request.CompareType != CompareWhat.Gas;
            if (compareElec)
            {
                _switchServiceHelper.UpdateItemData(usageTemplate, "elecUsageType", "usageType", UsageTypeBySpend);
                _switchServiceHelper.UpdateItemData(usageTemplate, "elecSpend", "usageAsSpend", request.SpendData.ElectricitySpendAmount.ToString());
                _switchServiceHelper.UpdateItemData(usageTemplate, "elecSpend", "spendPeriod", ((int)request.SpendData.ElectricitySpendPeriod).ToString());
            }
        }

        private void PopulateSpendUsageForGas(GetPricesRequest request, SwitchesApiResponse usageTemplate)
        {
            var compareGas = request.CompareType != CompareWhat.Electricity;
            if (compareGas)
            {
                _switchServiceHelper.UpdateItemData(usageTemplate, "gasUsageType", "usageType", UsageTypeBySpend);
                _switchServiceHelper.UpdateItemData(usageTemplate, "gasSpend", "usageAsSpend", request.SpendData.GasSpendAmount.ToString());
                _switchServiceHelper.UpdateItemData(usageTemplate, "gasSpend", "spendPeriod", ((int)request.SpendData.GasSpendPeriod).ToString());
            }
        }

        private void PopulateDetailedUsageForElectricity(GetPricesRequest request, SwitchesApiResponse usageTemplate)
        {
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecUsageType", "usageType", UsageTypeByEstimator);
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "houseType", request.EstimatorData.HouseType);
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "numberOfBedrooms", request.EstimatorData.NumberOfBedrooms);
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "mainCookingSource", request.EstimatorData.MainCookingSource);
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "cookingFrequency", ((int)CookingFrequency.Daily).ToString());
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "centralHeating", request.EstimatorData.MainHeatingSource);
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "numberOfOccupants", request.EstimatorData.NumberOfOccupants);
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "insulation", request.EstimatorData.HouseInsulation);
            _switchServiceHelper.UpdateItemData(usageTemplate, "elecDetailedEstimate", "energyUsage", request.EstimatorData.HouseOccupied);
        }

        private void PopulateDetailedUsageForGas(GetPricesRequest request, SwitchesApiResponse usageTemplate)
        {
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasUsageType", "usageType", UsageTypeByEstimator);
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "houseType", request.EstimatorData.HouseType);
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "numberOfBedrooms", request.EstimatorData.NumberOfBedrooms);
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "mainCookingSource", request.EstimatorData.MainCookingSource);
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "cookingFrequency", ((int)CookingFrequency.Daily).ToString());
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "centralHeating", request.EstimatorData.MainHeatingSource);
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "numberOfOccupants", request.EstimatorData.NumberOfOccupants);
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "insulation", request.EstimatorData.HouseInsulation);
            _switchServiceHelper.UpdateItemData(usageTemplate, "gasDetailedEstimate", "energyUsage", request.EstimatorData.HouseOccupied);
        }

        private SwitchesApiResponse GetPreferencesPostResponse(string url, GetPricesRequest request)
        {
            var preferencesTemplate = _switchServiceHelper.GetApiDataTemplate(url, PreferenceRel);

            PopulatePreferencesWithRequestData(preferencesTemplate, request);

            return _switchServiceHelper.GetSwitchesApiPostResponse(url, preferencesTemplate, PreferenceRel, _baseRequest);
        }

        private void PopulatePreferencesWithRequestData(SwitchesApiResponse preferencesTemplate, GetPricesRequest request)
        {
            const string filterOptionAll = "105";
            //const string resultsOrderPrice = "1";
            const string paymentMethodAny = "-1";

            _switchServiceHelper.UpdateItemData(preferencesTemplate, "tariffFilterOptions", "tariffFilterOption", filterOptionAll);
            //UpdateItemData(preferencesTemplate, "resultsOrder", "resultsOrder", resultsOrderPrice);

            if (request.PrePayment == "false")
                _switchServiceHelper.UpdateItemData(preferencesTemplate, "limitToPaymentType", "paymentMethod", paymentMethodAny);
        }

        private decimal GetCurrentAnnualSpend(FutureSupplies futureSuppliesResponse, GetPricesRequest request)
        {
            var currentBillValue = 0M;

            if (request.CompareType != CompareWhat.Electricity)
                currentBillValue += futureSuppliesResponse.Usage.Gas.AnnualSpend;

            if (request.CompareType != CompareWhat.Gas)
                currentBillValue += futureSuppliesResponse.Usage.Elec.AnnualSpend;

            return decimal.Round(currentBillValue, 0);
        }

        private UsageData GetEstimatedUsage(FutureSupplies futureSuppliesResponse)
        {
            var estimatedUsage = new UsageData();

            if (futureSuppliesResponse.CurrentSupply.Electricity != null)
                estimatedUsage.ElectricityKwh = futureSuppliesResponse.Usage.Elec.AnnualKWh;

            if (futureSuppliesResponse.CurrentSupply.Gas != null)
                estimatedUsage.GasKwh = futureSuppliesResponse.Usage.Gas.AnnualKWh;

            return estimatedUsage;
        }
    }
}
