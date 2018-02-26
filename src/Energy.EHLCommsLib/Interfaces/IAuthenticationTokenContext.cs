

using Energy.EHLCommsLib.Models.Http;

namespace Energy.EHLCommsLib.InterfacesHttp
{
    public interface IAuthenticationTokenContext
    {
        Token CurrentToken { get; }
        void GetNewToken();
        bool TokenExpired();
    }
}
