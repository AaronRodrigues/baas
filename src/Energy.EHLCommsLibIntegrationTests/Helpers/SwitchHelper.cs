using System;
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
    public class SwitchHelper 
    {
        private readonly ISwitchServiceClient _switchServiceClient;

        public SwitchHelper(ISwitchServiceClient switchServiceClient)
        {
            _switchServiceClient = switchServiceClient;
        }

        public void ApplyReference(SwitchesApiResponse response, string reference, string value)
        {
            var item = GetEhlItemForName(response, reference);
            if (item != null)
                item.Data = value;
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


        public SwitchesApiResponse GetApiDataTemplate(string url, string rel)
        {
            var response = _switchServiceClient.GetSwitchesApiGetResponse<SwitchesApiResponse>(url, rel);

            HandleResponse(response, url, "TEMPLATE GET");

            return response;
        }

        private void HandleResponse(ApiResponse response, string url, string action)
        {

            if (response == null) return;

            if (response.StatusCode == HttpStatusCode.InternalServerError && response.Errors != null)
            {
                Error internalServerError = response.Errors.FirstOrDefault(
                        o => o.Message != null && o.Message.Id == EhlErrorConstants.EhlErrorGeneric);

                if (internalServerError != null) throw new Exception();
            }


        }


        public bool SuccessfulResponseFromEhl(SwitchesApiResponse response)
        {
            return response.SuccessfulResponseFromEhl();
        }

    }
}
