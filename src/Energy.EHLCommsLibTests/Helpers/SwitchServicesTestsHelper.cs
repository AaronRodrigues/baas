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

                case "SwitchStatus_GetResponse":
                    fileName = @".\SwitchApiMessages\1-16-SwitchStatus-Get-Response.json";
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
                case "FutureSupplies-GetResponse":
                    fileName = @".\SwitchApiMessages\8-2-FutureSupplies-Get-Response.json";
                    break;

                default:
                    return string.Empty;
            }

            return File.ReadAllText(fileName);

        }

        #endregion
    }
}
