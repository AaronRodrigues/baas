using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Models.Http;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface ISwitchServiceClient
    {
        ISwitchServiceClient For(BaseRequest baseRequest);
        T GetSwitchesApiGetResponse<T>(string url, string relKey) where T : ApiResponse, new();
        T GetSwitchesApiPostResponse<T>(string url, T responseDataToSend, string relKey) where T : ApiResponse, new();
        object DownloadContentFor(string url, string contentType);
        HttpClientResponse GetApiResponse(string url);
    }
}