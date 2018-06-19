using Energy.EHLCommsLib.Contracts.Responses;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEhlHttpClient
    {
        T GetApiResponse<T>(string url, string environment) where T : ApiResponse;
        ApiResponse PostApiGetResponse(string url, ApiResponse responseDataToSend, string environment);
    }
}
