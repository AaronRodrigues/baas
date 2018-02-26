
using Energy.EHLCommsLibIntegrationTests.Model;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public interface IAuthorizationClient
    {
        IAuthorizationClient WithBaseUri(string baseUri);
        IAuthorizationClient WithResource(string resource);
        TokenResponse RequestToken(string userName, string password, string scope);
    }
}