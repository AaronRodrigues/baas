using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Interfaces;
using Newtonsoft.Json;

namespace Energy.EHLCommsLib.Http
{
    //TO DO : Add logging for exceptions and ehl errors
    public class EhlHttpClient : IEhlHttpClient
    {
        private const string ContentType = @"application/vnd-fri-domestic-energy+json;version=2.0";

        private readonly IPersistAttachments _attachmentPersister;
        private readonly HttpClient _httpClient;

        public EhlHttpClient(
            HttpMessageHandler messageHandler, 
            IPersistAttachments attachmentPersister)
        {
            _attachmentPersister = attachmentPersister;
            
            _httpClient = new HttpClient(messageHandler)
            {
                Timeout = TimeSpan.FromSeconds(45)
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<T> GetApiResponse<T>(string url, string environment) where T : ApiResponse
        {
            var request = HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);
            var responseBody = response.Content.ReadAsStringAsync().Result;
            
            SaveAttachment(environment, url, responseBody, "Get - Response");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public async Task<ApiResponse> PostApiGetResponse(string url, ApiResponse responseDataToSend, string environment)
        {
            var requestBody = RequestBody(responseDataToSend);

            SaveAttachment(environment, url, requestBody, "Post-Request");

            var httpRequestMessage = HttpRequestMessage(HttpMethod.Post, url);
            httpRequestMessage.Content = new StringContent(requestBody, Encoding.UTF8);
            httpRequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);

            var response = await _httpClient.SendAsync(httpRequestMessage);
            var responseBody = response.Content.ReadAsStringAsync().Result;

            SaveAttachment(environment, url, responseBody, "Post-Reponse");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ApiResponse>(responseBody);
        }

        private static string RequestBody(ApiResponse responseDataToSend)
        {
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            return JsonConvert.SerializeObject(responseDataToSend, Formatting.None, settings);
        }

        private void SaveAttachment(string environment, string url, string content, string description)
        {
            if (!IsProduction(environment))
            {
                _attachmentPersister?.Save(Attachment(url, content, description));
            }
        }

        private static bool IsProduction(string environment) 
        {
            return environment.Equals("prod", StringComparison.InvariantCultureIgnoreCase);
        }

        private static Attachment Attachment(string url, string responseBody, string method)
        {
            return new Attachment
            {
                Content = responseBody,
                Description = $"{method} - {url} - Body",
                MediaType = "application/json"
            };
        }

        public HttpRequestMessage HttpRequestMessage(HttpMethod httpMethod, string url)
        {
            var result = new HttpRequestMessage(httpMethod, url);
            result.Headers.Add("Accept", ContentType);
            return result;
        }
    }
}
