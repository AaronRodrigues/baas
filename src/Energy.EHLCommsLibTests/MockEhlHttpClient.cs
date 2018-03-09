using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Interfaces;
using Newtonsoft.Json;
using System.IO;

namespace Energy.EHLCommsLibTests
{
    class MockEntry
    {
        public MockEntry(string fileName, HttpStatusCode responseStatusCode, WebException exception)
        {
            this.fileName = fileName;
            this.responseStatusCode = responseStatusCode;
            this.exception = exception;
        }

        public string fileName { get; set; }
        public HttpStatusCode responseStatusCode { get; set; }
        public WebException exception { get; set; }
    }

    class MockEhlHttpClient : IEhlHttpClient
    {

        private Dictionary<string, MockEntry> mockMap_get = new Dictionary<string, MockEntry>();
        private Dictionary<string, MockEntry> mockMap_post = new Dictionary<string, MockEntry>();


        public T GetApiResponse<T>(string url) where T : ApiResponse, new()
        {
            var entry = GetEntryFor(mockMap_get, url);
            var json = GetJsonFor(entry.fileName);
            var result = JsonConvert.DeserializeObject<T>(json);
            result.StatusCode = entry.responseStatusCode;
            result.Exception = entry.exception;

            return result;
        }

        public ApiResponse PostApiGetResponse(string url, ApiResponse responseDataToSend)
        {
            var entry = GetEntryFor(mockMap_post, url);
            var json = GetJsonFor(entry.fileName);
            var result = JsonConvert.DeserializeObject<ApiResponse>(json);
            return result;
        }

        public MockEhlHttpClient Mock_GetApiResponse(string fileName, string urlFilter, HttpStatusCode responseStatusCode = HttpStatusCode.OK, WebException exception = null)
        {
            mockMap_get[urlFilter] = new MockEntry (fileName, responseStatusCode, exception);
            return this;
        }

        public MockEhlHttpClient Mock_PostSwitchesApiGetResponse(string fileName, string urlFilter)
        {
            mockMap_post[urlFilter] = new MockEntry(fileName, HttpStatusCode.OK, null);
            return this;
        }

        private MockEntry GetEntryFor(Dictionary<string, MockEntry> map, string url)
        {
            var entry = map.FirstOrDefault(r => url.Contains(r.Key));
            return entry.Value;
        }


        public string GetJsonFor(string fileName)
        {

            string filePath = $@".\SwitchApiMessages\{fileName}.json";
            var file = File.ReadAllText(filePath);
            return file;

        }

    }
}
