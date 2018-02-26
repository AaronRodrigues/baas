using System.Linq;
using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLibIntegrationTests.Model;

namespace Energy.EHLCommsLibIntegrationTests.Services
{
    public class StartSwitchService
    {
        private const string Ehl_ApiEntryPointUrl = "https://rest.staging.energyhelpline.com";
        private const string PartnerReference = "CTM123";

        private const string NextRel = "/rels/next";
        private const string StartSwitchUrl = "/domestic/energy/switches";
        private const string SwitchRel = "/rels/domestic/switch";
        private const string CurrentSupplyRel = "/rels/domestic/current-supply";
        private const string RegionRel = "/rels/domestic/region";
        private const string CurrentSuppliesRel = "/rels/domestic/current-supplies";

        private const string UnknownRegionId = "0";
        private const string NorthernIrelandRegionId = "16";

        private readonly ISwitchServiceHelper _switchServiceHelper;

        private BaseRequest _baseRequest;

        public StartSwitchService(ISwitchServiceHelper switchServiceHelper)
        {
            _switchServiceHelper = switchServiceHelper;
        }

        public StartSwitchResponse StartSwitch(StartSwitchRequest request)
        {
            var response = new StartSwitchResponse();
            var errorMessage = "Your postcode has not been recognised, please check you have entered it correctly and if you still need help call the switching support team on <b>0800&nbsp;093&nbsp;6831</b>.";

            _baseRequest = request;

            // Send postcode to EHL to start switch journey including validation and registration of postcode
            var registerResponse = RegisterPostcode(request.Postcode, request.ApiKey);

            if (registerResponse.Errors != null && registerResponse.Errors.Count > 0)
            {
                _switchServiceHelper.HydrateSwitchResponseWithErrors(response, registerResponse.Errors);
                return response;
            }

            if (registerResponse.NextStep().Is(CurrentSupplyRel))
            {
                if (NorthernIrelandPostcode(request.Postcode, registerResponse))
                    errorMessage = "Unfortunately this service is not available in Northern Ireland.";
                else
                    return GetStartSwitchResponse(registerResponse);
            }

            if (registerResponse.NextStep().Is(RegionRel))
            {
                SwitchesApiResponse regionTemplate;
                if (RegionHasBeenSuggested(registerResponse, out regionTemplate))
                {
                    // Get region data template
                    var regionUrl = registerResponse.Links.First(l => l.Rel.Contains(RegionRel) && l.Rel.Contains(NextRel)).Uri;
                    var regionConfirmationResponse = GetRegionConfirmationResponse(regionUrl, regionTemplate);

                    if (regionConfirmationResponse.NextStep().Is(CurrentSupplyRel))
                    {
                        return GetStartSwitchResponse(regionConfirmationResponse);
                    }
                }
            }

            response.Messages.Add(new Message { Type = MessageType.Error, Text = errorMessage });
            response.Success = false;

            return response;
        }

        private SwitchesApiResponse RegisterPostcode(string postcode, string apiKey)
        {
            var url = Ehl_ApiEntryPointUrl + StartSwitchUrl;
            var switchTemplate = _switchServiceHelper.GetApiDataTemplate(url, SwitchRel);

            // Add data template info
            _switchServiceHelper.ApplyReference(switchTemplate, "partnerReference", PartnerReference);
            _switchServiceHelper.ApplyReference(switchTemplate, "apiKey", apiKey);
            switchTemplate.DataTemplate.Groups[0].Items[0].Data = postcode;

            return _switchServiceHelper.GetSwitchesApiPostResponse(url, switchTemplate, SwitchRel, _baseRequest);
        }

        private bool NorthernIrelandPostcode(string postCode, SwitchesApiResponse registerResponse)
        {
            if (postCode.ToLower().StartsWith("bt"))
            {
                var switchUrl = registerResponse.Links.First(l => l.Rel.Contains(SwitchRel)).Uri;
                var switchStatus = _switchServiceHelper.GetSwitchesApiGetResponse<SwitchApiResponse>(switchUrl, SwitchRel, _baseRequest);
                if (switchStatus != null)
                    return switchStatus.SupplyLocation.Region.Id.Equals(NorthernIrelandRegionId);
            }

            return false;
        }

        private StartSwitchResponse GetStartSwitchResponse(SwitchesApiResponse ehlResponse)
        {
            var switchId = GetSwitchId(ehlResponse);
            _baseRequest.SwitchId = switchId;

            // Get current supply template
            var currentSupplyUrl = ehlResponse.Links.First(l => l.Rel.Contains(CurrentSupplyRel) && l.Rel.Contains(NextRel)).Uri;
            var currentSupplyResponse = _switchServiceHelper.GetApiDataTemplate(currentSupplyUrl, CurrentSupplyRel);

            var response = MapToEnergyResponse(currentSupplyResponse);
            response.SwitchId = switchId;
            response.PostCode = _baseRequest.Postcode;

            return response;
        }

        private string GetSwitchId(SwitchesApiResponse ehlResponse)
        {
            var switchUrl = ehlResponse.Links.First(l => l.Rel.Contains(SwitchRel)).Uri;
            var switchStatus = _switchServiceHelper.GetSwitchesApiGetResponse<SwitchApiResponse>(switchUrl, SwitchRel, _baseRequest);
            return switchStatus != null ? switchStatus.Id : string.Empty;
        }

        private StartSwitchResponse MapToEnergyResponse(SwitchesApiResponse ehlResponse)
        {
            var response = new StartSwitchResponse
            {
                CompareGas = (bool)_switchServiceHelper.GetEhlItemForName(ehlResponse, "compareGas").Data,
                CompareElectricity = (bool)_switchServiceHelper.GetEhlItemForName(ehlResponse, "compareElec").Data
            };

            var group = _switchServiceHelper.GetEhlGroupForName(ehlResponse, "gasTariff");
            response.DefaultGasSupplierId = int.Parse(_switchServiceHelper.GetEhlItemForName(group, "supplier").Data.ToString());
            response.DefaultGasSupplierTariffId = int.Parse(_switchServiceHelper.GetEhlItemForName(group, "supplierTariff").Data.ToString());
            response.DefaultGasPaymentMethod = (string)_switchServiceHelper.GetEhlItemForName(group, "paymentMethod").Data;

            group = _switchServiceHelper.GetEhlGroupForName(ehlResponse, "elecTariff");
            response.DefaultElecSupplierId = int.Parse(_switchServiceHelper.GetEhlItemForName(group, "supplier").Data.ToString());
            response.DefaultElecSupplierTariffId = int.Parse(_switchServiceHelper.GetEhlItemForName(group, "supplierTariff").Data.ToString());
            response.DefaultElecPaymentMethod = (string)_switchServiceHelper.GetEhlItemForName(group, "paymentMethod").Data;
            response.DefaultElecEconomy7 = (bool)_switchServiceHelper.GetEhlItemForName(group, "economy7").Data;

            response.SwitchStatusUrl = ehlResponse.Links.First(l => l.Rel.Equals(SwitchRel)).Uri;
            response.CurrentSupplyUrl = ehlResponse.Links.First(l => l.Rel.Contains(CurrentSupplyRel)).Uri;
            response.CurrentSuppliersUrl = ehlResponse.LinkedDataSources.First(d => d.Rel.Equals(CurrentSuppliesRel)).Uri;

            return response;
        }

        private bool RegionHasBeenSuggested(SwitchesApiResponse registerResponse, out SwitchesApiResponse regionTemplate)
        {
            var regionUrl = registerResponse.Links.First(l => l.Rel.Contains(RegionRel)).Uri;
            regionTemplate = _switchServiceHelper.GetApiDataTemplate(regionUrl, RegionRel);
            var region = regionTemplate.DataTemplate.Groups[0].Items[0].Data;
            return region != null && !region.Equals(UnknownRegionId);
        }

        private SwitchesApiResponse GetRegionConfirmationResponse(string url, SwitchesApiResponse regionTemplate)
        {
            return _switchServiceHelper.GetSwitchesApiPostResponse(url, regionTemplate, RegionRel, _baseRequest);
        }
    }
}