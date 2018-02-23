namespace Energy.EHLCommsLib.Interfaces
{
    public interface IHttpClient
    {
        T Get<T>(string url);
        T Get<T>(string url, string postData);
    }
}