using System.Collections.Generic;
using Energy.EHLCommsLib.Contracts;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Models;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface ISwitchServiceHelper
    {
        void ApplyReference(ApiResponse response, string reference, string value);
        Item GetEhlItemForName(ApiResponse response, string name);
        Item GetEhlItemForName(Group group, string name);
        Group GetEhlGroupForName(ApiResponse response, string name);
        void UpdateItemData(ApiResponse currentSupplyTemplate, string groupName, string itemName, string value);
        void HydrateSwitchResponseWithErrors(BaseResponse response, IEnumerable<Error> ehlErrors);
        ApiResponse GetApiDataTemplate(string url);
        string GetLinkedDataUrl(ApiResponse response, string rel);
        T GetSwitchesApiGetResponse<T>(string url, string relKey, BaseRequest request) where T : ApiResponse, new();

        T GetSwitchesApiPostResponse<T>(string url, T responseDataToSend, string relKey, BaseRequest request)
            where T : ApiResponse, new();
    }
}