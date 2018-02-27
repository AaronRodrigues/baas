namespace Energy.EHLCommsLib.Interfaces.Http
{
    public interface IHttpClient
    {
        string ContentType { get; set; }
        string AcceptHeader { get; set; }
        string AuthorizationToken { get; set; }
        void AddHeader(string key, string value);
        IResponse Get(string requestUrl);
        IResponse Post(string requestUrl, string postData);
        IHttpClient WithTimeOutOf(int milliseconds);
        IHttpClient WithoutRedirect();
    }
}
