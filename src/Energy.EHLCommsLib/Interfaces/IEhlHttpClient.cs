using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Models;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEhlHttpClient
    {
        SwitchesApiResponse GetApiDataTemplate(string url, string rel);

        //T GetSwitchesApiPostResponse<T>(string url, T responseDataToSend, string relKey, BaseRequest request)
        //   where T : ApiResponse, new();

        //T GetSwitchesApiGetResponse<T>(string url, string relKey, BaseRequest request) where T : ApiResponse, new();
    }
}
