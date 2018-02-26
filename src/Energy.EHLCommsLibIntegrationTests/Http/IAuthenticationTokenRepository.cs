using Energy.EHLCommsLib.Models.Http;
using Energy.EHLCommsLibIntegrationTests.Model;

namespace Energy.EHLCommsLibIntegrationTests.Http { 

    public interface IAuthenticationTokenRepository
    {
        int UpdateAuthenticationTokenAudit(Token token);
    }
}