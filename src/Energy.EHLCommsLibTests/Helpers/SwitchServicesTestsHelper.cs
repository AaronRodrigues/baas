using System;
using System.Net;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Http;
using System.IO;
using Rhino.Mocks;

namespace Energy.EHLCommsLibTests.Helpers
{
    public class SwitchServicesTestsHelper
    {

        public SwitchServicesTestsHelper Mock_ApiGetRequest(IHttpClientWrapper httpClientWrapper, string jsonKey, string urlFilter, HttpStatusCode? responseStatusCode = null, WebException exception = null)
        {
            httpClientWrapper.Expect(
                c =>
                c.CallGet(Arg<HttpClientRequest>.Matches(r => r.Url.Contains(urlFilter))))
                             .Return(new HttpClientResponse
                             {
                                 Data = GetJsonFor(jsonKey),
                                 ResponseStatusCode = responseStatusCode,
                                 Exception = exception
                             });
            return this;
        }

        public SwitchServicesTestsHelper Mock_ApiPostRequest(IHttpClientWrapper httpClientWrapper, string jsonKey, string urlFilter)
        {
            httpClientWrapper.Expect(
                c =>
                c.CallPost(Arg<HttpClientRequest>.Matches(r => r.Url.Contains(urlFilter))))
                             .Return(new HttpClientResponse
                             {
                                 Data = GetJsonFor(jsonKey)
                             });
            return this;
        }

        #region Read switch messages from files

        public string GetJsonFor(string key)
        {
            string fileName;

            switch (key)
            {
                case "StartSwitch_GetResponse":
                    fileName = @".\SwitchApiMessages\1-2-StartSwitch-Get-Response.json";
                    break;

                case "StartSwitch_PostResponse":
                    fileName = @".\SwitchApiMessages\1-4-StartSwitch-Post-Response-ValidRegion.json";
                    break;

                case "StartSwitch_PostResponse_InvalidPostcode":
                    fileName = @".\SwitchApiMessages\1-6-StartSwitch-Post-Response-InvalidPostcode.json";
                    break;

                case "StartSwitch_PostResponse_ChannelIslandPostcode":
                    fileName = @".\SwitchApiMessages\1-7-StartSwitch-Post-Response-ChannelIslandPostcode.json";
                    break;

                case "SwitchStatus_GetResponse_NorthernIreland":
                    fileName = @".\SwitchApiMessages\1-17-SwitchStatus-NorthernIrelandRegion-Get-Response.json";
                    break;

                case "StartSwitch_PostResponse_RegionSuggested":
                    fileName = @".\SwitchApiMessages\1-5-StartSwitch-Post-Response-RegionSuggested.json";
                    break;

                case "StartSwitch_RegionSuggested_GetResponse":
                    fileName = @".\SwitchApiMessages\1-12-StartSwitch-RegionSuggested-Get-Response.json";
                    break;

                case "StartSwitch_RegionSuggested_PostResponse":
                    fileName = @".\SwitchApiMessages\1-14-StartSwitch-RegionSuggested-Post-Response.json";
                    break;

                case "SwitchStatus_GetResponse":
                    fileName = @".\SwitchApiMessages\1-16-SwitchStatus-Get-Response.json";
                    break;

                case "SwitchStatus_GetResponse_ConfirmedSwitch":
                    fileName = @".\SwitchApiMessages\1-18-SwitchStatus-Get-Response-ConfirmedSwitch.json";
                    break;

                case "SwitchStatus_GetResponse_NoProRataUrl":
                    fileName = @".\SwitchApiMessages\1-19-SwitchStatus-Get-Response-NoProRataUrl.json";
                    break;

                case "CurrentSupply_GetResponse":
                    fileName = @".\SwitchApiMessages\2-2-CurrentSupply-Get-Response.json";
                    break;

                case "CurrentSupply-GetResponse-WithError-AlreadySwitched":
                    fileName = @".\SwitchApiMessages\2-3-CurrentSupply-Get-Response-WithError-AlreadySwitched.json";
                    break;

                case "CurrentSupply-GetResponse-WithError-InternalServerError":
                    fileName = @".\SwitchApiMessages\2-4-CurrentSupply-Get-Response-WithError-InternalServerError.json";
                    break;

                case "CurrentSupplies-GetResponse":
                    fileName = @".\SwitchApiMessages\3-2-CurrentSupplies-Get-Response.json";
                    break;

                case "TariffDetails-GetResponse-DualFuelTariff":
                    fileName = @".\SwitchApiMessages\9-2-TariffDetails-Get-Response-DualFuelTariff.json";
                    break;

                case "TariffDetails-GetResponse-GasOnlyTariff":
                    fileName = @".\SwitchApiMessages\9-3-TariffDetails-Get-Response-GasOnlyTariff.json";
                    break;

                case "TariffDetails-GetResponse-ElectricityOnlyTariff":
                    fileName = @".\SwitchApiMessages\9-4-TariffDetails-Get-Response-ElectricityOnlyTariff.json";
                    break;

                case "TariffDetails-GetResponse-DualFuelTariff-NoTermsLink":
                    fileName = @".\SwitchApiMessages\9-5-TariffDetails-Get-Response-DualFuelTariff-With-NoSupplierTermsLink.json";
                    break;

                case "CurrentSupply_PostResponse":
                    fileName = @".\SwitchApiMessages\4-2-CurrentSupply-Post-Response.json";
                    break;

                case "CurrentSupply_PostResponse_WithErrors":
                    fileName = @".\SwitchApiMessages\4-3-CurrentSupply-Post-Response-WithErrors.json";
                    break;

                case "ProRata-GetResponse":
                    fileName = @".\SwitchApiMessages\4-5-ProRata-Get-Response.json";
                    break;

                case "ProRata-PostResponse":
                    fileName = @".\SwitchApiMessages\4-6-ProRata-Post-Response.json";
                    break;

                case "Usage-GetResponse":
                    fileName = @".\SwitchApiMessages\5-2-Usage-Get-Response.json";
                    break;

                case "Usage-PostResponse":
                    fileName = @".\SwitchApiMessages\5-4-Usage-Post-Response.json";
                    break;

                case "Usage-PostResponse-WithError-NegativeElecUsage":
                    fileName = @".\SwitchApiMessages\5-5-Usage-Post-Response-WithErrors-NegativeElecUsage.json";
                    break;

                case "Usage-PostResponse-WithError-NegativeGasUsage":
                    fileName = @".\SwitchApiMessages\5-6-Usage-Post-Response-WithErrors-NegativeGasUsage.json";
                    break;

                case "Preferences-GetResponse":
                    fileName = @".\SwitchApiMessages\6-2-Preferences-Get-Response.json";
                    break;

                case "Preferences-PostResponse":
                    fileName = @".\SwitchApiMessages\6-4-Preferences-Post-Response.json";
                    break;

                case "FutureSupply-GetResponse":
                    fileName = @".\SwitchApiMessages\7-2-FutureSupply-Get-Response.json";
                    break;

                case "FutureSupply-GetResponse-WithErrors":
                    fileName = @".\SwitchApiMessages\7-3-FutureSupply-Get-Response-WithErrors.json";
                    break;

                case "FutureSupplies-GetResponse":
                    fileName = @".\SwitchApiMessages\8-2-FutureSupplies-Get-Response.json";
                    break;

                case "FutureSupply-PostResponse":
                    fileName = @".\SwitchApiMessages\10-2-FutureSupply-Post-Response.json";
                    break;

                case "FutureSupply-PostResponse-WithErrors":
                    fileName = @".\SwitchApiMessages\10-3-FutureSupply-Post-Response-WithErrors.json";
                    break;

                case "PrepareForTransfer-GetResponse":
                    fileName = @".\SwitchApiMessages\11-2-PrepareForTransfer-Get-Response.json";
                    break;

                case "PrepareForTransfer-PostResponse":
                    fileName = @".\SwitchApiMessages\11-4-PrepareForTransfer-Post-Response.json";
                    break;

                case "HeldApplicationConfirmationFeed":
                    fileName = @".\SwitchApiMessages\12-1-HeldApplicationConfirmation-Feed.json";
                    break;

                case "NonHeldApplicationConfirmationFeed":
                    fileName = @".\SwitchApiMessages\12-2-NonHeldApplicationConfirmation-Feed.json";
                    break;

                case "Signup-GetResponse":
                    fileName = @".\SwitchApiMessages\13-2-Signup-Get-Response.json";
                    break;

                case "Signup-GetResponse-WithErrors":
                    fileName = @".\SwitchApiMessages\13-3-Signup-Get-Response-WithErrors.json";
                    break;

                case "Signup-PostResponse-WithErrors":
                    fileName = @".\SwitchApiMessages\13-4-Signup-Post-Response-WithErrors.json";
                    break;

                case "Signup-PostResponse":
                    fileName = @".\SwitchApiMessages\13-5-Signup-Post-Response.json";
                    break;

                case "Signup-PostResponse-SwitchBlocked":
                    fileName = @".\SwitchApiMessages\13-6-Signup-SwitchBlocked-Post-Response.json";
                    break;

                default:
                    return string.Empty;
            }

            return File.ReadAllText(fileName);

        }

        #endregion
    }
}
