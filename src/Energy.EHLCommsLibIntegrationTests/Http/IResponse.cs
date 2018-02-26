using System.Net;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public interface IResponse
    {
        HttpStatusCode StatusCode { get; }
        string StatusDescription { get; }
        string BodyAsString();
        byte[] BodyAsBinary();
    }
}
