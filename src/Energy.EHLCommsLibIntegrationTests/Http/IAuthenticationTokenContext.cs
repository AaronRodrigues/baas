
using Energy.EHLCommsLibIntegrationTests.Model;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public interface IAuthenticationTokenContext
    {
        Token CurrentToken { get; }
        void GetNewToken();
        bool TokenExpired();
    }
}
