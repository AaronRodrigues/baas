using System.Collections.Generic;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Contracts;

namespace Energy.EHLCommsLibIntegrationTests.Interfaces
{
    public interface ISwitchServiceHelper
    {
        void ApplyReference(SwitchesApiResponse response, string reference, string value);
        Item GetEhlItemForName(ApiResponse response, string name);
        Item GetEhlItemForName(Group group, string name);
        Group GetEhlGroupForName(SwitchesApiResponse response, string name);
        void UpdateItemData(SwitchesApiResponse currentSupplyTemplate, string groupName, string itemName, string value);
        void HydrateSwitchResponseWithErrors(BaseResponse response, IEnumerable<Error> ehlErrors);
        SwitchesApiResponse GetApiDataTemplate(string url, string rel);
        string GetLinkedDataUrl(SwitchesApiResponse response, string rel);
        T GetSwitchesApiGetResponse<T>(string url, string relKey, BaseRequest request) where T : ApiResponse, new();

        T GetSwitchesApiPostResponse<T>(string url, T responseDataToSend, string relKey, BaseRequest request)
            where T : ApiResponse, new();
        bool SuccessfulResponseFromEhl(SwitchesApiResponse response);

    }
}
