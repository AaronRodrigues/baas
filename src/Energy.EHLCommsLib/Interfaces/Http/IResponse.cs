using System.Net;

namespace Energy.EHLCommsLib.Interfaces.Http
{
    public interface IResponse
    {
        HttpStatusCode StatusCode { get; }
        string StatusDescription { get; }
        string BodyAsString();
        byte[] BodyAsBinary();
    }
}
