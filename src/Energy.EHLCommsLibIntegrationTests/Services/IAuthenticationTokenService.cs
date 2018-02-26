using Energy.EHLCommsLibIntegrationTests.Model;

namespace Energy.EHLCommsLibIntegrationTests.Services
{
    public interface IAuthenticationTokenService
    {
        GenerateTokenResponse GenerateNew();
    }
}