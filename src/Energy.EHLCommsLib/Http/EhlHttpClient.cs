using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.External.Exceptions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Http;
using Newtonsoft.Json;
using Message = Energy.EHLCommsLib.Contracts.Common.Message;

namespace Energy.EHLCommsLib.Http
{
    //TO DO : Add logging for exceptions and ehl errors
    public class EhlHttpClient : IEhlHttpClient
    {
        private const string ContentType = @"application/vnd-fri-domestic-energy+json;version=2.0";
        public EhlHttpClient()
        {
        }

        public T GetApiResponse<T>(string url, string journeyid) where T : ApiResponse, new()
        {
            var apiResponse = ApiGet(url);
            return HandleRequest<T>(apiResponse, url, journeyid);
        }

        public ApiResponse PostSwitchesApiGetResponse(string url, ApiResponse responseDataToSend, string journeyid)
        {
            var dataToPost = JsonConvert.SerializeObject(responseDataToSend.DataTemplate);
            var apiResponse = ApiPost(url, dataToPost);
            return HandleRequest<ApiResponse>(apiResponse, url, journeyid);
        }
       

        private T HandleRequest<T>(IResponse apiResponse, string url, string journeyid) where T : ApiResponse, new()
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
                response = new T
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Exception = new WebException("The remote server returned an error: (500) Internal Server Error."),
                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Message = new Message
                            {
                                Id = EhlErrorConstants.EhlErrorGeneric,
                                Text = "Internal server error. Reference:"
                            }
                        }
                    }
                };
                LogError(url, "TEMPLATE GET", journeyid);
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

        private void LogError(string action, string url, string journeyId)
        {
            var logInfo = string.Format("Invalid switch encountered while making a {0} request to EHL using url {1}. JourneyId = {2}", action, url, journeyId);
            //Log.Info(logInfo);
            var message = string.Format(
                "Internal server error received from EHL\nMessage = 'Internal server error. Reference:', JourneyId = '{0}', Action='{1}', Url='{2}'",
                journeyId,
                action,
                url);
            throw new InvalidSwitchException(message, new WebException("The remote server returned an error: (500) Internal Server Error."));
        }

    }
}
