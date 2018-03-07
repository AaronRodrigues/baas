using Energy.EHLCommsLib.Contracts.Responses;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEhlHttpClient
    {
        T GetApiResponse<T>(string url) where T : ApiResponse, new();
        ApiResponse PostApiGetResponse(string url, ApiResponse responseDataToSend);
    }
}
