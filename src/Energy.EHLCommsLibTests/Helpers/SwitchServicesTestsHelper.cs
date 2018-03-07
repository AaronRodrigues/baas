using System;
using System.Net;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Http;
using System.IO;
using Energy.EHLCommsLib.Contracts.Responses;
using Rhino.Mocks;
using Newtonsoft.Json;

namespace Energy.EHLCommsLibTests.Helpers
{
    public class SwitchServicesTestsHelper
    {

        public SwitchServicesTestsHelper Mock_ApiGetRequest(IEhlHttpClient ehlHttpClient, string jsonKey, string urlFilter, HttpStatusCode? responseStatusCode = null, WebException exception = null)
        {
            ehlHttpClient.Expect(
                c =>
                    c.GetApiResponse<ApiResponse>(Arg<String>.Matches(r => r.Contains(urlFilter)), Arg<String>.Matches(r => true)))
                .Return(JsonConvert.DeserializeObject<ApiResponse>(GetJsonFor(jsonKey)));

            return this;
        }

        public SwitchServicesTestsHelper Mock_PostSwitchesApiGetResponse(IEhlHttpClient ehlHttpClient, string jsonKey, string urlFilter)
        {
            ehlHttpClient.Expect(
                c =>
                    c.PostSwitchesApiGetResponse(Arg<String>.Matches(r => r.Contains(urlFilter)), Arg<ApiResponse>.Matches(r => true), Arg<String>.Matches(r => true)))

                .Return(JsonConvert.DeserializeObject<ApiResponse>(GetJsonFor(jsonKey)));

            return this;
        }

        
        public string GetJsonFor(string key)
        {
            string fileName = $@".\SwitchApiMessages\{key}.json";
            var file = File.ReadAllText(fileName);
            return file;

        }

    }
}
