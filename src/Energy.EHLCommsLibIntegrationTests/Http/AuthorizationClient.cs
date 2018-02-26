using System.Net;
using Energy.EHLCommsLibIntegrationTests.Model;
using RestSharp;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public class AuthorizationClient : IAuthorizationClient
    {
        private const string OAuthMediaType = "application/x-www-form-urlencoded";

        private string _baseUrl;
        private string _tokenEndpoint;

        public IAuthorizationClient WithBaseUri(string baseUri) //"https://rest.staging.energyhelpline.com"
        {
            _baseUrl = baseUri;
            return this;
        }

        public IAuthorizationClient WithResource(string resource)
        {
            _tokenEndpoint = resource;
            return this;
        }

        public TokenResponse RequestToken(string userName, string password, string scope)
        {
            var authorizationServer = new RestClient
            {
                BaseUrl = _baseUrl,
                Authenticator = new HttpBasicAuthenticator(userName, password),
            };

            //var proxy = AppSettings.WebClientProxy;
            //if (!string.IsNullOrWhiteSpace(proxy))
            //    authorizationServer.Proxy = new WebProxy(proxy);

            var request = new RestRequest
            {
                Resource = _tokenEndpoint,
                Method = Method.POST,
            };

            request.AddParameter(OAuthMediaType, "grant_type=client_credentials&scope=" + scope, ParameterType.RequestBody);

            // TokenResponse object contains the access token, if successful, or an error code and OPTIONAL error description if unsuccessful.
            var response = authorizationServer.Execute<TokenResponse>(request);

            return response.Data;
        }
    }
}