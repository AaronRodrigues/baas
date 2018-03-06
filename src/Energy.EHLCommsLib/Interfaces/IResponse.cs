using System.Net;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IResponse
    {
        HttpStatusCode StatusCode { get; }
        string StatusDescription { get; }
        string BodyAsString();
        byte[] BodyAsBinary();
    }
}
