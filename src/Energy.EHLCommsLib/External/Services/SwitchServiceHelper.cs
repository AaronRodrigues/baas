using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.External.Exceptions;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Interfaces.Http;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Models.Http;
using Newtonsoft.Json;
using Message = Energy.EHLCommsLib.Models.Message;

namespace Energy.EHLCommsLib.External.Services
{
    //TO DO: Move this logic to some external library because it used in tests only
    public class SwitchServiceHelper : ISwitchServiceHelper
    {
        private readonly ISwitchServiceClient _switchServiceClient;
        private readonly IHttpClient _httpClient;

        public SwitchServiceHelper(ISwitchServiceClient switchServiceClient, IHttpClient httpClient)
        {
            _switchServiceClient = switchServiceClient;
            _httpClient = httpClient;
        }

        public void ApplyReference(ApiResponse response, string reference, string value)
        {
            var item = GetEhlItemForName(response, reference);
            if (item != null)
                item.Data = value;
        }

        public string GetLinkedDataUrl(ApiResponse response, string rel)
        {
            return response.LinkedDataSources
                .First(l => l.Rel.Equals(rel))
                .Uri;
        }

        public T GetSwitchesApiGetResponse<T>(string url, string relKey, BaseRequest request)
            where T : ApiResponse, new()
        {
            var response = _switchServiceClient.GetSwitchesApiGetResponse<T>(url);

            HandleResponse(response, url, "GET");

            return response;
        }

        public T GetSwitchesApiPostResponse<T>(string url, T responseDataToSend, string relKey, BaseRequest request)
            where T : ApiResponse, new()
        {
            var response = _switchServiceClient.GetSwitchesApiPostResponse(url, responseDataToSend, relKey);

            HandleResponse(response, url, "POST");

            return response;
        }

        public Item GetEhlItemForName(ApiResponse response, string name)
        {
            Item item = null;
            foreach (var group in response.DataTemplate.Groups)
            {
                item = group.Items.FirstOrDefault(i => i.Name.Equals(name));
                if (item != null)
                    break;
            }
            return item;
        }

        public Item GetEhlItemForName(Group group, string name)
        {
            return group.Items.FirstOrDefault(i => i.Name.Equals(name));
        }

        public Group GetEhlGroupForName(ApiResponse response, string name)
        {
            return response.DataTemplate.Groups.FirstOrDefault(@group => group.Name.Equals(name));
        }

        public void UpdateItemData(ApiResponse currentSupplyTemplate, string groupName, string itemName,
            string value)
        {
            currentSupplyTemplate.DataTemplate.Groups
                .First(g => g.Name.Equals(groupName))
                .Items.First(i => i.Name.Equals(itemName))
                .Data = value;
        }

        public void HydrateSwitchResponseWithErrors(BaseResponse response, IEnumerable<Error> ehlErrors)
        {
            if (ehlErrors != null)
            {
                foreach (var ehlError in ehlErrors)
                {
                    var code = MapEhlMessageIdToMessageCode(ehlError.Message.Id);
                    response.Messages.Add(new Message
                    {
                        Code = MapEhlMessageIdToMessageCode(ehlError.Message.Id),
                        Item = ehlError.Item,
                        Text = MapEhlToCtmMessageText(code, ehlError.Message.Text),
                        Type = MessageType.Error
                    });
                }
            }

            response.ResponseStatusType = DetermineResponseStatusType(response);

            response.Success = false;
        }

        public ApiResponse GetApiDataTemplate(string url)
        {
            var response = GetSwitchesApiGetResponse(url);

            HandleResponse(response, url, "TEMPLATE GET");

            return response;
        }

        private ApiResponse GetSwitchesApiGetResponse(string url)
        {
            var response = GetApiResponse(url);

            ApiResponse responseObject;

            try
            {
                responseObject = JsonConvert.DeserializeObject<ApiResponse>(response.Data.ToString());
                responseObject.StatusCode = response.ResponseStatusCode;
                responseObject.Exception = response.Exception;
            }
            catch (Exception)
            {
                //Log.Error(string.Format("EHL ERROR - EHL returned invalid JSON from a GET to Url '{0}'. Most likely they returned the HTML of their error page.", apiUrl));

                responseObject = InternalServerError<ApiResponse>();
            }

            return responseObject;
        }

        private HttpClientResponse GetApiResponse(string url)
        {
            var proxyRequest = new HttpClientRequest
            {
                Url = url.Replace("http://", "https://"),
                ContentType = "application/vnd-fri-domestic-energy+json;version=2.0"
            };
            return CallGet(proxyRequest, response => response.BodyAsString(), "application/vnd-fri-domestic-energy+json;version=2.0");
        }

        private HttpClientResponse CallGet(HttpClientRequest request, Func<IResponse, object> downloadAction, string acceptHeader)
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

        private static T InternalServerError<T>() where T : ApiResponse, new()
        {
            return new T
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Exception = new WebException("The remote server returned an error: (500) Internal Server Error."),
                Errors = new List<Error>
                {
                    new Error
                    {
                        Message = new Contracts.Common.Message
                        {
                            Id = EhlErrorConstants.EhlErrorGeneric,
                            Text = "Internal server error. Reference:"
                        }
                    }
                }
            };
        }

        private MessageCode DetermineResponseStatusType(BaseResponse response)
        {
            if (response.Messages.Any(m => m.Code.Equals(MessageCode.AlreadySwitched)))
                return MessageCode.AlreadySwitched;

            if (response.Messages.Any(m => m.Code.Equals(MessageCode.NegativeElectricityUsage)))
                return MessageCode.NegativeElectricityUsage;

            if (response.Messages.Any(m => m.Code.Equals(MessageCode.NegativeGasUsage)))
                return MessageCode.NegativeGasUsage;

            if (response.Messages.Any(m => m.Code.Equals(MessageCode.ChannelIslandPostcodeEntered)))
                return MessageCode.ChannelIslandPostcodeEntered;

            if (!response.Messages.Any() || response.Messages.Any(m => m.Code.Equals(MessageCode.InternalServerError)))
                return MessageCode.InternalServerError;

            return MessageCode.Unknown;
        }

        private MessageCode MapEhlMessageIdToMessageCode(string ehlMessageId)
        {
            switch (ehlMessageId)
            {
                case EhlErrorConstants.EhlErrorCodeChannelIsland:
                    return MessageCode.ChannelIslandPostcodeEntered;
                case EhlErrorConstants.EhlErrorOperationNotAvailable:
                    return MessageCode.AlreadySwitched;
                case EhlErrorConstants.EhlErrorGeneric:
                    return MessageCode.InternalServerError;
                case EhlErrorConstants.EhlErrorNegativeElecUsage:
                    return MessageCode.NegativeElectricityUsage;
                case EhlErrorConstants.EhlErrorNegativeGasUsage:
                    return MessageCode.NegativeGasUsage;
                default:
                    return MessageCode.Unknown;
            }
        }

        private string MapEhlToCtmMessageText(MessageCode code, string text)
        {
            return code == MessageCode.ChannelIslandPostcodeEntered ? "Unfortunately this service is not available outside of the UK mainland." : text;
        }

        private void HandleResponse(ApiResponse response, string url, string action)
        {
            // TODO: When EHL provide a more specific error for an invalid switch this code should be updated

            if (response == null)
            {
                return;
            }

            Error internalServerError = null;

            if (response.StatusCode == HttpStatusCode.InternalServerError && response.Errors != null)
            {
                internalServerError =
                    response.Errors.FirstOrDefault(
                        o => o.Message != null && o.Message.Id == EhlErrorConstants.EhlErrorGeneric);
            }

            if (internalServerError != null)
            {
                if (response.Exception != null &&
                    response.Exception.Message.StartsWith(
                        "The remote server returned an error: (500) Internal Server Error."))
                {
                    var ehlMessage = internalServerError.Message.Text;

                    if (ehlMessage != null && ehlMessage.StartsWith("Internal server error. Reference:"))
                    {
                        var builder = new StringBuilder();

                        builder.AppendFormat(
                            "Invalid switch encountered while making a {0} request to EHL using url {1}.", action, url);
                        builder.AppendLine();
                        //builder.AppendFormat("JourneyId = {0}", _applicationContext.JourneyId);
                        builder.AppendLine();

                        //Log.Info(builder.ToString());

                        var message = string.Format(
                            "Internal server error received from EHL\nMessage = '{0}', JourneyId = '{1}', Action='{2}', Url='{3}'",
                            ehlMessage,
                            0,//_applicationContext.JourneyId,
                            action,
                            url);

                        throw new InvalidSwitchException(message, response.Exception);
                    }
                }
            }

            //LogResponseErrors(response, url, action);
        }
    }
}