namespace Energy.EHLCommsLib.Models.Http
{
    public class HttpClientRequest
    {
        public string WebProxyAddress { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }
        public string ContentType { get; set; }
        public string AuthorizationToken { get; set; }
    }
}