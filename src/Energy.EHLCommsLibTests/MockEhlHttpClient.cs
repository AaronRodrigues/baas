using System.Collections.Generic;
using System.Linq;
using System.Net;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Exceptions;

namespace Energy.EHLCommsLibTests
{
   public class MockEhlHttpClient : IEhlHttpClient
    {
        private readonly Dictionary<string, MockEntry> _mockMapGet = new Dictionary<string, MockEntry>();
        private readonly Dictionary<string, MockEntry> _mockMapPost = new Dictionary<string, MockEntry>();

        public Task<T> GetApiResponse<T>(string url, string environment) where T : ApiResponse
        {
            var entry = GetEntryFor(_mockMapGet, url);
            var json = GetJsonFor(entry.FileName);
            var result = JsonConvert.DeserializeObject<T>(json);
            result.StatusCode = entry.ResponseStatusCode;
            result.Exception = entry.Exception;

            if (result.Exception != null)
            {
                throw new InvalidSwitchException("", result.Exception);
            }

            return Task.FromResult(result);
        }

        public Task<ApiResponse> PostApiGetResponse(string url, ApiResponse responseDataToSend, string environment)
        {
            var entry = GetEntryFor(_mockMapPost, url);
            var json = GetJsonFor(entry.FileName);
            var result = JsonConvert.DeserializeObject<ApiResponse>(json);
            return Task.FromResult(result);
        }

        public MockEhlHttpClient Mock_GetApiResponse(string fileName, string urlFilter, HttpStatusCode responseStatusCode = HttpStatusCode.OK, WebException exception = null)
        {
            _mockMapGet[urlFilter] = new MockEntry (fileName, responseStatusCode, exception);
            return this;
        }

        public MockEhlHttpClient Mock_PostSwitchesApiGetResponse(string fileName, string urlFilter)
        {
            _mockMapPost[urlFilter] = new MockEntry(fileName, HttpStatusCode.OK, null);
            return this;
        }

        private MockEntry GetEntryFor(Dictionary<string, MockEntry> map, string url)
        {
            var entry = map.FirstOrDefault(r => url.Contains(r.Key));
            return entry.Value;
        }


        public string GetJsonFor(string fileName)
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string filePath = Path.Combine(basePath, "SwitchApiMessages",$"{fileName}.json");
            
            var file = File.ReadAllText(filePath);
            return file;

        }

    }
}
