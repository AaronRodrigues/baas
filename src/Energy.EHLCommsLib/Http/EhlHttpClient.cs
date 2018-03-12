using System;
using System.Net;
using System.Text;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Exceptions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Http;
using Newtonsoft.Json;

namespace Energy.EHLCommsLib.Http
{
    //TO DO : Add logging for exceptions and ehl errors
    public class EhlHttpClient : IEhlHttpClient
    {
        private const string ContentType = @"application/vnd-fri-domestic-energy+json;version=2.0";

        public T GetApiResponse<T>(string url) where T : ApiResponse, new()
        {
            var apiResponse = ApiGet(url);
            return HandleRequest<T>(apiResponse, url, "GET");
        }

        public ApiResponse PostApiGetResponse(string url, ApiResponse responseDataToSend)
        {
            var dataToPost = JsonConvert.SerializeObject(responseDataToSend.DataTemplate);
            var apiResponse = ApiPost(url, dataToPost);
            return HandleRequest<ApiResponse>(apiResponse, url, "POST");
        }
       

        private T HandleRequest<T>(IResponse apiResponse, string url, string action) where T : ApiResponse, new()
        {
            T response;
            try
            {
                response = JsonConvert.DeserializeObject<T>(apiResponse.BodyAsString());
                response.StatusCode = apiResponse.StatusCode;
                response.Exception = response.Exception;
            }
            catch (Exception)
            {
                var message = string.Format(
                "Internal server error received from EHL\nMessage = 'Internal server error. Reference:', Action='{0}', Url='{1}'",
                action,
                url);
                throw new InvalidSwitchException(message, new WebException("The remote server returned an error: (500) Internal Server Error."));
            }
            //Add logging
            return response;
        }

        private IResponse ApiPost(string url, string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(replaceProtocol(url));
            request.Method = "POST";
            request.ContentType = ContentType;
            request.Accept = ContentType;
            using (var stream = request.GetRequestStream())
            {
                var requestBody = Encoding.ASCII.GetBytes(data);
                stream.Write(requestBody, 0, requestBody.Length);
            }

            return new HttpResponse((HttpWebResponse)request.GetResponse());
        }

        private IResponse ApiGet(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(replaceProtocol(url));
            request.ContentType = ContentType;
            request.Accept = ContentType;
            return new HttpResponse((HttpWebResponse)request.GetResponse());
        }

        private string replaceProtocol(string url)
        {
            return url.Replace("http://", "https://");
        }

    }
}
