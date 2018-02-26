namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class GenerateTokenResponse
    {
        public bool Success { get; set; }

        public string Scope { get; set; }
        public string TokenType { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }

        public Token Token { get; set; }
    }
}