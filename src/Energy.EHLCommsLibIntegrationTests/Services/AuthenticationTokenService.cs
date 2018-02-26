using System;
using Energy.EHLCommsLib.Models.Http;
using Energy.EHLCommsLibIntegrationTests.Http;
using Energy.EHLCommsLibIntegrationTests.Model;


namespace Energy.EHLCommsLibIntegrationTests.Services
{
    public class AuthenticationTokenService : IAuthenticationTokenService
    {
        private readonly IAuthorizationClient _authorizationClient;
        //private readonly IAuthenticationTokenRepository _tokenRepository;
        private const string DomesticScope = "Domestic";

        public AuthenticationTokenService(IAuthorizationClient authorizationClient) //, IAuthenticationTokenRepository tokenRepository)
        {
            _authorizationClient = authorizationClient;
//            _tokenRepository = tokenRepository;
        }

        public GenerateTokenResponse GenerateNew()
        {
            var response = new GenerateTokenResponse();

            try
            {
                var tokenResponse = _authorizationClient.WithBaseUri("https://rest.staging.energyhelpline.com").WithResource("/token")
                    .RequestToken(Environment.GetEnvironmentVariable("ehl_authentication_token_username"),
                                  Environment.GetEnvironmentVariable("ehl_authentication_token_password"), 
                                  DomesticScope);

                if (tokenResponse != null)
                {
                    response.TokenType = tokenResponse.TokenType;
                    response.Scope = tokenResponse.Scope;
                    response.Error = tokenResponse.Error;
                    response.ErrorDescription = tokenResponse.ErrorDescription;
                    if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken) && string.IsNullOrWhiteSpace(tokenResponse.Error))
                    {
                        response.Token = new Token
                                             {
                                                 AccessToken = tokenResponse.AccessToken,
                                                 ExpiresIn = tokenResponse.ExpiresIn,
                                                 ExpiryTime = SystemTime.Now.AddSeconds(tokenResponse.ExpiresIn)
                                             };
  //                      _tokenRepository.UpdateAuthenticationTokenAudit(response.Token);
                        response.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error("Error while generating/persisting authorisation token.", ex);
            }

            return response;
        }
    }
}