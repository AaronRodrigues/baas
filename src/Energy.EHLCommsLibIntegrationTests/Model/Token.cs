using System;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class Token
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}