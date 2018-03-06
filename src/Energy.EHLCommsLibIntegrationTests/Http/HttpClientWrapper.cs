using System;
using System.IO;
using System.Net;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Http;


namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IHttpClient _httpClient;

        public HttpClientWrapper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClientResponse CallPost(HttpClientRequest request)
        {
            var response = new HttpClientResponse();
                _httpClient.ContentType = request.ContentType;
                _httpClient.AuthorizationToken = request.AuthorizationToken;

                var httpResponse = _httpClient.Post(request.Url, request.Data);
                response.ResponseStatusCode = httpResponse?.StatusCode;
                response.Data = httpResponse?.BodyAsString();


            return response;
        }

        public HttpClientResponse CallGet(HttpClientRequest request)
        {
            return DownloadContent(request, response => response.BodyAsString(), "application/vnd-fri-domestic-energy+json;version=2.0");
        }

        public HttpClientResponse GetBinaryContent(HttpClientRequest request)
        {
            return DownloadContent(request, response => response.BodyAsBinary(), string.Empty);
        }

        private HttpClientResponse DownloadContent(HttpClientRequest request, Func<IResponse, object> downloadAction, string acceptHeader)
        {
            var response = new HttpClientResponse();
                _httpClient.ContentType = request.ContentType;
                _httpClient.AuthorizationToken = request.AuthorizationToken;
                _httpClient.AcceptHeader = acceptHeader;

                var httpResponse = _httpClient.Get(request.Url);
                response.ResponseStatusCode = httpResponse != null ? httpResponse.StatusCode : (HttpStatusCode?)null;
                response.Data = downloadAction(httpResponse);

            
            return response;
        }


    }
}
