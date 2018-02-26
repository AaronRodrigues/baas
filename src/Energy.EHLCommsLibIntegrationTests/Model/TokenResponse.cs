namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string Scope { get; set; }
        public string TokenType { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
    }
}