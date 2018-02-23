using System.Net;

namespace Energy.EHLCommsLib.Models.Http
{
    public class HttpClientResponse
    {
        public object Data { get; set; }
        public WebException Exception { get; set; }
        public HttpStatusCode? ResponseStatusCode { get; set; }
    }
}