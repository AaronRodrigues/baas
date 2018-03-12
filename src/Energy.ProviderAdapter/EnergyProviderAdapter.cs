using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CTM.Quoting.Provider;

namespace Energy.ProviderAdapter
{
    //TO DO: Add data for quoting integration
    public class EnergyProviderAdapter : IProviderAdapter<EnergyEnquiry, EnergyQuote>
    {
        private readonly string providerName;
        private readonly string brandCodePrefix;

        public EnergyProviderAdapter(string providerName, string brandCodePrefix)
        {
            this.providerName = providerName;
            this.brandCodePrefix = brandCodePrefix;
        }

        public Task<QuoteResult<EnergyQuote>> GetQuotes(MakeProviderEnquiry<EnergyEnquiry> providerEnquiry)
        {
            return Task.FromResult(
                new QuoteResult<EnergyQuote>
                {
                    NonQuotes = new List<NonQuote>
                    {
                        new NonQuote
                        {
                            Brand = brandCodePrefix + "3",
                            Reason = Reason.Error,
                            Note = $"Error from {providerName}, enquiryId: {providerEnquiry.Enquiry.Id}"
                        }
                    },
                    Quotes = new List<EnergyQuote>
                    {
                        new EnergyQuote
                        {
                            Brand = brandCodePrefix + "1",
                            Quote = 12.35m,
                            EnquiryId = providerEnquiry.Enquiry.Id,
                            Note = $"Quote 1 of 2 from {providerName}"
                        },
                        new EnergyQuote
                        {
                            Brand = brandCodePrefix + "2",
                            Quote = 34.99m,
                            EnquiryId = providerEnquiry.Enquiry.Id,
                            Note = $"Quote 2 of 2 from {providerName}"
                        }
                    }
                });
        }
    }
}