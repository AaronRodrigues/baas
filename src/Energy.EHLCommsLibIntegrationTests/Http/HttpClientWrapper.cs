﻿using System;
using System.IO;
using System.Net;
using CTM.Energy.Common.Interfaces;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Http;


namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IHttpClient _httpClient;
        //private readonly IPublishMetricEvents _metricsEventsPublisher;

        public HttpClientWrapper(IHttpClient httpClient)//, IPublishMetricEvents metricsEventsPublisher)
        {
            _httpClient = httpClient;
            //_metricsEventsPublisher = metricsEventsPublisher;
        }

        public HttpClientResponse CallPost(HttpClientRequest request)
        {
            var response = new HttpClientResponse();
            try
            {
                _httpClient.ContentType = request.ContentType;
                _httpClient.AuthorizationToken = request.AuthorizationToken;

                var watch = System.Diagnostics.Stopwatch.StartNew();

                var httpResponse = _httpClient.Post(request.Url, request.Data);
                response.ResponseStatusCode = httpResponse != null ? httpResponse.StatusCode : (HttpStatusCode?)null;
                response.Data = httpResponse != null ? httpResponse.BodyAsString() : null;

                watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //_metricsEventsPublisher.Publish(new ApiRequestEvent { TargetService = "ehl", Path = RemoveQueryStringFromUrl(request.Url), HttpVerb = "POST", HttpStatusCode = response.ResponseStatusCode.ToString(), Value = elapsedMs });

            }
            catch (WebException webEx)
            {
                response.Exception = webEx;
                response.Data = GetWebExceptionResponseStream(webEx);
                response.ResponseStatusCode = GetHttpStatusCode(webEx);
                //_metricsEventsPublisher.Publish(new ApiRequestEvent { TargetService = "ehl", Path = RemoveQueryStringFromUrl(request.Url)
                //    , HttpVerb = "POST", HttpStatusCode = response.ResponseStatusCode.ToString() });
            }
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
            try
            {
                _httpClient.ContentType = request.ContentType;
                _httpClient.AuthorizationToken = request.AuthorizationToken;
                _httpClient.AcceptHeader = acceptHeader;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var httpResponse = _httpClient.Get(request.Url);
                response.ResponseStatusCode = httpResponse != null ? httpResponse.StatusCode : (HttpStatusCode?)null;
                response.Data = downloadAction(httpResponse);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                //_metricsEventsPublisher.Publish(new ApiRequestEvent { TargetService = "ehl", Path = RemoveQueryStringFromUrl(request.Url), HttpVerb = "GET", HttpStatusCode = response.ResponseStatusCode.ToString(), Value = elapsedMs });
            }
            catch (WebException webEx)
            {
                response.Exception = webEx;
                response.Data = GetWebExceptionResponseStream(webEx);
                response.ResponseStatusCode = GetHttpStatusCode(webEx);
                //_metricsEventsPublisher.Publish(new ApiRequestEvent { TargetService = "ehl", Path = RemoveQueryStringFromUrl(request.Url), HttpVerb = "GET", HttpStatusCode = response.ResponseStatusCode.ToString()});
            }
            catch (Exception ex)
            {
                //Log.Error(string.Format("Exception while making GET request to {0}.", request.Url), ex);
                //_metricsEventsPublisher.Publish(new ApiRequestEvent { TargetService = "ehl", Path = RemoveQueryStringFromUrl(request.Url) });
                throw;
            }
            
            return response;
        }

        private HttpStatusCode? GetHttpStatusCode(WebException webEx)
        {
            var httpResponse = webEx.Response as HttpWebResponse;
            return httpResponse != null ? httpResponse.StatusCode : (HttpStatusCode?)null;
        }

        private string GetWebExceptionResponseStream(WebException webEx)
        {
            if (webEx.Response != null)
            {
                using (var stream = webEx.Response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd().Trim();
                        }
                    }
                }
            }

            return null;
        }

        private string RemoveQueryStringFromUrl(string url)
        {
            var uriScheme = new Uri(url).Scheme;
            var uriHost = new Uri(url).Host;
            var uriPath = new Uri(url).LocalPath;
            var convertedUrl = uriScheme + "://" + uriHost + uriPath;
            return convertedUrl;
        }
    }
}
