using System;
using System.Collections.Generic;
using System.Net;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Http;
using Newtonsoft.Json;

namespace Energy.EHLCommsLib.External.Services
{
    //TO DO: Move this logic to some external library because it used in tests only
    public class SwitchServiceClient : ISwitchServiceClient
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public SwitchServiceClient(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public T GetSwitchesApiGetResponse<T>(string url, string relKey) where T : ApiResponse, new()
        {
           var response = GetApiResponse(url);

            T responseObject;

            try
            {
                responseObject = JsonConvert.DeserializeObject<T>(response.Data.ToString());
                responseObject.StatusCode = response.ResponseStatusCode;
                responseObject.Exception = response.Exception;
            }
            catch (Exception)
            {
                //Log.Error(string.Format("EHL ERROR - EHL returned invalid JSON from a GET to Url '{0}'. Most likely they returned the HTML of their error page.", apiUrl));

                responseObject = InternalServerError<T>();
            }

            return responseObject;
        }

        public T GetSwitchesApiPostResponse<T>(string url, T responseDataToSend, string relKey) where T : ApiResponse, new()
        {
            var dataToPost = JsonConvert.SerializeObject(responseDataToSend.DataTemplate);

            var apiUrl = string.Format("{0}{1}", "", url);

            var response = PostJsonTo(apiUrl, dataToPost);

            T responseObject;

            try
            {
                responseObject = JsonConvert.DeserializeObject<T>(response.Data.ToString());
                responseObject.StatusCode = response.ResponseStatusCode;
                responseObject.Exception = response.Exception;
            }
            catch (Exception)
            {
                //Log.Error(string.Format("EHL ERROR - EHL returned invalid JSON from a POST to Url '{0}'. Most likely they returned the HTML of their error page. The JSON that was posted was: {1}",
                //    apiUrl,
                //    dataToPost));

                responseObject = InternalServerError<T>();
            }

            return responseObject;
        }

        public object DownloadContentFor(string url, string contentType)
        {
            return GetContentFor(url, contentType);
        }

        public HttpClientResponse GetApiResponse(string url)
        {
            var proxyRequest = CreateNewHttpRequest(url);

            return _httpClientWrapper.CallGet(proxyRequest);
        }

        private object GetContentFor(string url, string contentType)
        {
            try
            {
                var proxyRequest = CreateNewHttpRequest(url);
                proxyRequest.ContentType = contentType;

                var response = _httpClientWrapper.GetBinaryContent(proxyRequest);

                if (response.Exception != null)
                {
                    //Log.Error(string.Format("Web exception while making GET request to {0}.", url), response.Exception);
                }

                return response.Data;
            }
            catch (Exception ex)
            {
                //Log.Error(string.Format("Unable to download content from Api for url - {0}", url), ex);
                return null;
            }
        }

        private HttpClientResponse PostJsonTo(string url, string data)
        {
            var proxyRequest = CreateNewHttpRequest(url);
            proxyRequest.Data = data;

            var response = _httpClientWrapper.CallPost(proxyRequest);

            return response;
        }

        private HttpClientRequest CreateNewHttpRequest(string url)
        {
            // Ensure that we always use https with Api 3
            url = url.Replace("http://", "https://");

            var request = new HttpClientRequest
            {
                Url = url,
                ContentType = "application/vnd-fri-domestic-energy+json;version=2.0",
                WebProxyAddress = ""
            };

            return request;
        }

        public static T InternalServerError<T>() where T : ApiResponse, new()
        {
            return new T
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Exception = new WebException("The remote server returned an error: (500) Internal Server Error."),
                Errors = new List<Error>
                {
                    new Error
                    {
                        //Message = new Energy.EHLCommsLib.Contracts.Common.Error.Message
                        //{
                        //    Id = EhlErrorConstants.EhlErrorGeneric,
                        //    Text = "Internal server error. Reference:"
                        //}
                    }
                }
            };
        }
    }
}
