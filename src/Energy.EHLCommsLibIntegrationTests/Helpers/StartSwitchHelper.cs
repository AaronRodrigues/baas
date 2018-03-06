using System.Linq;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Extensions;

using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLibIntegrationTests.Model;


namespace Energy.EHLCommsLibIntegrationTests.Services
{
    public class StartSwitchHelper
    {
        private const string EhlApiEntryPointUrl = "https://rest.staging.energyhelpline.com";
        private const string PartnerReference = "CTM123";
        private const string TestPostcode = "pe26ys";
        private readonly string ApiKey = System.Environment.GetEnvironmentVariable("ehl_api_key");

        private const string NextRel = "/rels/next";
        private const string StartSwitchUrl = "/domestic/energy/switches";
        private const string SwitchRel = "/rels/domestic/switch";
        private const string CurrentSupplyRel = "/rels/domestic/current-supply";
        private const string RegionRel = "/rels/domestic/region";
        private const string CurrentSuppliesRel = "/rels/domestic/current-supplies";

        private const string UnknownRegionId = "0";

        private readonly SwitchHelper _switchHelper;

        private BaseRequest _baseRequest;

        public StartSwitchHelper(SwitchHelper switchHelper)
        {
            _switchHelper = switchHelper;
        }

        public StartSwitchResponse StartSwitch()
        {
            var response = new StartSwitchResponse();

            _baseRequest = new StartSwitchRequest {Postcode = TestPostcode, ApiKey = ApiKey};

            var registerResponse = RegisterPostcode(TestPostcode, ApiKey);

            if (registerResponse.NextStep().Is(CurrentSupplyRel)) return GetStartSwitchResponse(registerResponse);

            if (registerResponse.NextStep().Is(RegionRel))
            {
                ApiResponse regionTemplate;
                if (RegionHasBeenSuggested(registerResponse, out regionTemplate))
                {
                    var regionUrl = registerResponse.Links.First(l => l.Rel.Contains(RegionRel) && l.Rel.Contains(NextRel)).Uri;
                    var regionConfirmationResponse = GetRegionConfirmationResponse(regionUrl, regionTemplate);

                    if (regionConfirmationResponse.NextStep().Is(CurrentSupplyRel)) return GetStartSwitchResponse(regionConfirmationResponse);
                }
            }


            return response;
        }

        private ApiResponse RegisterPostcode(string postcode, string apiKey)
        {
            const string url = EhlApiEntryPointUrl + StartSwitchUrl;
            var switchTemplate = _switchHelper.GetApiDataTemplate(url, SwitchRel);

            // Add data template info
            _switchHelper.ApplyReference(switchTemplate, "partnerReference", PartnerReference);
            _switchHelper.ApplyReference(switchTemplate, "apiKey", apiKey);
            switchTemplate.DataTemplate.Groups[0].Items[0].Data = postcode;

            return _switchHelper.GetSwitchesApiPostResponse(url, switchTemplate, SwitchRel, _baseRequest);
        }

        private StartSwitchResponse GetStartSwitchResponse(ApiResponse ehlResponse)
        {
            var switchId = GetSwitchId(ehlResponse);
            _baseRequest.SwitchId = switchId;

            var currentSupplyUrl = ehlResponse.Links.First(l => l.Rel.Contains(CurrentSupplyRel) && l.Rel.Contains(NextRel)).Uri;
            var currentSupplyResponse = _switchHelper.GetApiDataTemplate(currentSupplyUrl, CurrentSupplyRel);

            var response = MapToEnergyResponse(currentSupplyResponse);
            response.SwitchId = switchId;
            response.PostCode = _baseRequest.Postcode;

            return response;
        }

        private string GetSwitchId(ApiResponse ehlResponse)
        {
            var switchUrl = ehlResponse.Links.First(l => l.Rel.Contains(SwitchRel)).Uri;
            var switchStatus = _switchHelper.GetSwitchesApiGetResponse<SwitchApiResponse>(switchUrl, SwitchRel, _baseRequest);
            return switchStatus != null ? switchStatus.Id : string.Empty;
        }

        private StartSwitchResponse MapToEnergyResponse(ApiResponse ehlResponse)
        {
            var response = new StartSwitchResponse
            {
                CompareGas = (bool)_switchHelper.GetEhlItemForName(ehlResponse, "compareGas").Data,
                CompareElectricity = (bool)_switchHelper.GetEhlItemForName(ehlResponse, "compareElec").Data
            };

            var group = _switchHelper.GetEhlGroupForName(ehlResponse, "gasTariff");
            response.DefaultGasSupplierId = int.Parse(_switchHelper.GetEhlItemForName(group, "supplier").Data.ToString());
            response.DefaultGasSupplierTariffId = int.Parse(_switchHelper.GetEhlItemForName(group, "supplierTariff").Data.ToString());
            response.DefaultGasPaymentMethod = (string)_switchHelper.GetEhlItemForName(group, "paymentMethod").Data;

            group = _switchHelper.GetEhlGroupForName(ehlResponse, "elecTariff");
            response.DefaultElecSupplierId = int.Parse(_switchHelper.GetEhlItemForName(group, "supplier").Data.ToString());
            response.DefaultElecSupplierTariffId = int.Parse(_switchHelper.GetEhlItemForName(group, "supplierTariff").Data.ToString());
            response.DefaultElecPaymentMethod = (string)_switchHelper.GetEhlItemForName(group, "paymentMethod").Data;
            response.DefaultElecEconomy7 = (bool)_switchHelper.GetEhlItemForName(group, "economy7").Data;

            response.SwitchStatusUrl = ehlResponse.Links.First(l => l.Rel.Equals(SwitchRel)).Uri;
            response.CurrentSupplyUrl = ehlResponse.Links.First(l => l.Rel.Contains(CurrentSupplyRel)).Uri;
            response.CurrentSuppliersUrl = ehlResponse.LinkedDataSources.First(d => d.Rel.Equals(CurrentSuppliesRel)).Uri;

            return response;
        }

        private bool RegionHasBeenSuggested(ApiResponse registerResponse, out ApiResponse regionTemplate)
        {
            var regionUrl = registerResponse.Links.First(l => l.Rel.Contains(RegionRel)).Uri;
            regionTemplate = _switchHelper.GetApiDataTemplate(regionUrl, RegionRel);
            var region = regionTemplate.DataTemplate.Groups[0].Items[0].Data;
            return region != null && !region.Equals(UnknownRegionId);
        }

        private ApiResponse GetRegionConfirmationResponse(string url, ApiResponse regionTemplate)
        {
            return _switchHelper.GetSwitchesApiPostResponse(url, regionTemplate, RegionRel, _baseRequest);
        }
    }
}