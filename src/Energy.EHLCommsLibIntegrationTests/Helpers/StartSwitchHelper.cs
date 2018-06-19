using System.Linq;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Contracts;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.Http;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLibIntegrationTests.Helpers;
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
        private const string Environment = "environment";


        private const string UnknownRegionId = "0";

        private IEhlHttpClient _ehlHttpClient;

        public StartSwitchHelper(IEhlHttpClient ehlHttpClient)
        {
            _ehlHttpClient = ehlHttpClient;
        }

        public StartSwitchResponse StartSwitch()
        {
            var response = new StartSwitchResponse();

            var registerResponse = RegisterPostcode(TestPostcode, ApiKey);

            if (registerResponse.NextStep().Is(CurrentSupplyRel)) return GetStartSwitchResponse(registerResponse);

            if (!registerResponse.NextStep().Is(RegionRel)) return response;
            ApiResponse regionTemplate;
            if (!RegionHasBeenSuggested(registerResponse, out regionTemplate)) return response;
            var regionUrl = registerResponse.Links.First(l => l.Rel.Contains(RegionRel) && l.Rel.Contains(NextRel)).Uri;
            var regionConfirmationResponse = _ehlHttpClient.PostApiGetResponse(regionUrl, regionTemplate, Environment);

            if (regionConfirmationResponse.NextStep().Is(CurrentSupplyRel)) return GetStartSwitchResponse(regionConfirmationResponse);


            return response;
        }

        private ApiResponse RegisterPostcode(string postcode, string apiKey)
        {
            const string url = EhlApiEntryPointUrl + StartSwitchUrl;
            var switchTemplate = _ehlHttpClient.GetApiResponse<ApiResponse>(url, Environment);

            ApplyReference(switchTemplate, "partnerReference", PartnerReference);
            ApplyReference(switchTemplate, "apiKey", apiKey);
            switchTemplate.DataTemplate.Groups[0].Items[0].Data = postcode;

            return _ehlHttpClient.PostApiGetResponse(url, switchTemplate, Environment);
        }

        private StartSwitchResponse GetStartSwitchResponse(ApiResponse ehlResponse)
        {
            var switchId = GetSwitchId(ehlResponse);

            var currentSupplyUrl = ehlResponse.Links.First(l => l.Rel.Contains(CurrentSupplyRel) && l.Rel.Contains(NextRel)).Uri;
            var currentSupplyResponse = _ehlHttpClient.GetApiResponse<ApiResponse>(currentSupplyUrl, Environment);

            var response = MapToEnergyResponse(currentSupplyResponse);
            response.SwitchId = switchId;
            response.PostCode = TestPostcode;

            return response;
        }

        private string GetSwitchId(ApiResponse ehlResponse)
        {
            var switchUrl = ehlResponse.Links.First(l => l.Rel.Contains(SwitchRel)).Uri;
            var switchStatus = _ehlHttpClient.GetApiResponse<SwitchApiResponse>(switchUrl, Environment);
            return switchStatus != null ? switchStatus.Id : string.Empty;
        }

        private StartSwitchResponse MapToEnergyResponse(ApiResponse ehlResponse)
        {
            var response = new StartSwitchResponse
            {
                CompareGas = (bool)GetEhlItemForName(ehlResponse, "compareGas").Data,
                CompareElectricity = (bool)GetEhlItemForName(ehlResponse, "compareElec").Data
            };

            var group = GetEhlGroupForName(ehlResponse, "gasTariff");
            response.DefaultGasSupplierId = int.Parse(GetEhlItemForName(group, "supplier").Data.ToString());
            response.DefaultGasSupplierTariffId = int.Parse(GetEhlItemForName(group, "supplierTariff").Data.ToString());
            response.DefaultGasPaymentMethod = (string)GetEhlItemForName(group, "paymentMethod").Data;

            group = GetEhlGroupForName(ehlResponse, "elecTariff");
            response.DefaultElecSupplierId = int.Parse(GetEhlItemForName(group, "supplier").Data.ToString());
            response.DefaultElecSupplierTariffId = int.Parse(GetEhlItemForName(group, "supplierTariff").Data.ToString());
            response.DefaultElecPaymentMethod = (string)GetEhlItemForName(group, "paymentMethod").Data;
            response.DefaultElecEconomy7 = (bool)GetEhlItemForName(group, "economy7").Data;

            response.SwitchStatusUrl = ehlResponse.Links.First(l => l.Rel.Equals(SwitchRel)).Uri;
            response.CurrentSupplyUrl = ehlResponse.Links.First(l => l.Rel.Contains(CurrentSupplyRel)).Uri;
            response.CurrentSuppliersUrl = ehlResponse.LinkedDataSources.First(d => d.Rel.Equals(CurrentSuppliesRel)).Uri;

            return response;
        }

        private bool RegionHasBeenSuggested(ApiResponse registerResponse, out ApiResponse regionTemplate)
        {
            var regionUrl = registerResponse.Links.First(l => l.Rel.Contains(RegionRel)).Uri;
            regionTemplate = _ehlHttpClient.GetApiResponse<ApiResponse>(regionUrl, Environment);
            var region = regionTemplate.DataTemplate.Groups[0].Items[0].Data;
            return region != null && !region.Equals(UnknownRegionId);
        }

        private Group GetEhlGroupForName(ApiResponse response, string name) => response.DataTemplate.Groups.FirstOrDefault(@group => @group.Name.Equals(name));

        private Item GetEhlItemForName(Group group, string name)
        {
            return group.Items.FirstOrDefault(i => i.Name.Equals(name));
        }

        private void ApplyReference(ApiResponse response, string reference, string value)
        {
            var item = GetEhlItemForName(response, reference);
            if (item != null)
                item.Data = value;
        }

        private Item GetEhlItemForName(ApiResponse response, string name)
        {
            Item item = null;
            foreach (var group in response.DataTemplate.Groups)
            {
                item = group.Items.FirstOrDefault(i => i.Name.Equals(name));
                if (item != null)
                    break;
            }
            return item;
        }
    }
}