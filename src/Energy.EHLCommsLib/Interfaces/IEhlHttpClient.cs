using System.Threading.Tasks;
using Energy.EHLCommsLib.Contracts.Responses;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEhlHttpClient
    {
        Task<T> GetApiResponse<T>(string url, string environment) where T : ApiResponse;
        Task<ApiResponse> PostApiGetResponse(string url, ApiResponse responseDataToSend, string environment);
    }
}
