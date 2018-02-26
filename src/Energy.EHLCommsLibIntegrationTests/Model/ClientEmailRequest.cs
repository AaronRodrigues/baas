using System;
using System.Collections.Generic;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class ClientEmailRequest
    {
        public Guid ResultsKey { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NumberOfQuotes { get; set; }
        public List<EmailPriceResult> FilteredPriceResults { get; set; }
    }
}
