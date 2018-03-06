using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Models;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEhlHttpClient
    {
        T GetApiResponse<T>(string url, string journeyid) where T : ApiResponse, new();
        ApiResponse PostSwitchesApiGetResponse(string url, ApiResponse responseDataToSend,
            string journeyid);
    }
}
