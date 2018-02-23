using Energy.EHLCommsLib.Models.Http;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IHttpClientWrapper
    {
        HttpClientResponse CallPost(HttpClientRequest request);
        HttpClientResponse CallGet(HttpClientRequest request);
        HttpClientResponse GetBinaryContent(HttpClientRequest request);
    }
}