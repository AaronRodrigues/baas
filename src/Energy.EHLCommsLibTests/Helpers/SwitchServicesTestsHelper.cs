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
            string fileName = $@".\SwitchApiMessages\{key}.json";
            return File.ReadAllText(fileName);

        }

        #endregion
    }
}
