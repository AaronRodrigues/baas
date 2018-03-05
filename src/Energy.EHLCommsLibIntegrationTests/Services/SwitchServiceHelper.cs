using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.External.Exceptions;
using Energy.EHLCommsLib.Models;
using Message = Energy.EHLCommsLib.Models.Message;
using Energy.EHLCommsLib.Contracts;
using Energy.EHLCommsLib.Interfaces;

namespace Energy.EHLCommsLibIntegrationTests.Services
{
    public class SwitchServiceHelper : ISwitchServiceHelper
    {
        private readonly ISwitchServiceClient _switchServiceClient;

        public SwitchServiceHelper(ISwitchServiceClient switchServiceClient)
        {
            _switchServiceClient = switchServiceClient;
        }

        public void ApplyReference(SwitchesApiResponse response, string reference, string value)
        {
            var item = GetEhlItemForName(response, reference);
            if (item != null)
                item.Data = value;
        }

        public string GetLinkedDataUrl(SwitchesApiResponse response, string rel)
        {
            return response.LinkedDataSources
                .First(l => l.Rel.Equals(rel))
                .Uri;
        }

        public T GetSwitchesApiGetResponse<T>(string url, string relKey, BaseRequest request)
            where T : ApiResponse, new()
        {
            var response = _switchServiceClient.GetSwitchesApiGetResponse<T>(url, relKey);

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

        public Group GetEhlGroupForName(SwitchesApiResponse response, string name)
        {
            return response.DataTemplate.Groups.FirstOrDefault(@group => group.Name.Equals(name));
        }

        public void UpdateItemData(SwitchesApiResponse currentSupplyTemplate, string groupName, string itemName,
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

        public SwitchesApiResponse GetApiDataTemplate(string url, string rel)
        {
            var response = _switchServiceClient.GetSwitchesApiGetResponse<SwitchesApiResponse>(url, rel);

            HandleResponse(response, url, "TEMPLATE GET");

            return response;
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
            if (code == MessageCode.ChannelIslandPostcodeEntered)
                return "Unfortunately this service is not available outside of the UK mainland.";

            return text;
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

            LogResponseErrors(response, url, action);
        }

        private void LogResponseErrors(ApiResponse response, string url, string action)
        {
            if (response.Exception == null && (response.Errors == null || !response.Errors.Any()))
            {
                return;
            }

            var builder = new StringBuilder();

            builder.AppendFormat("Issue occurred while making a {0} request to EHL using url {1}.", action, url);
            builder.AppendLine();
            //builder.AppendFormat("JourneyId = {0}", _applicationContext.JourneyId);
            builder.AppendLine();
            builder.AppendFormat("Status code returned was {0}", response.StatusCode);
            builder.AppendLine();

            if (response.Errors != null)
            {
                foreach (var error in response.Errors)
                {
                    builder.AppendFormat("Response includes message '{0}' with Id of '{1}'", error.Message.Text,
                        error.Message.Id);
                    builder.AppendLine();
                }
            }

            var logLevel = response.IsExpectedError() ? MessageType.Warning : MessageType.Error;

            var message = builder.ToString();

            if (response.Exception != null)
            {
                //Log.LogException(logLevel, message, response.Exception);
            }
        }

        public bool SuccessfulResponseFromEhl(SwitchesApiResponse response)
        {
            return response.SuccessfulResponseFromEhl();
        }

    }
}
