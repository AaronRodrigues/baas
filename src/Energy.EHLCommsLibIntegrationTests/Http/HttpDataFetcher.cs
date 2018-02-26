using System;
using System.Net;
using CTM.Energy.Common.Interfaces;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public class HttpDataFetcher : IDataFetcher
    {
        private readonly IHttpClient _httpClient;

        public HttpDataFetcher(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string FetchFrom(string url)
        {
            var response = _httpClient.Get(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.BodyAsString();
            }

            throw new Exception(url);
        }

        public IDataFetcher UsingTimeOutOf(int timeOutValueInMilliseconds)
        {
            _httpClient.WithTimeOutOf(timeOutValueInMilliseconds);
            return this;
        }
    }
}
