using System.Collections.Generic;
using System.Threading.Tasks;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Http;
using Energy.ProviderAdapter.Models;

namespace Energy.ProviderAdapter
{
    //TO DO: Logging
    //TO DO: Update EnergyEnquery with required fields
    //To DO; Error handling
    public class EnergyProviderAdapter : IProviderAdapter<EnergyEnquiry, EnergyQuote>
    {
        private readonly string providerName;
        private readonly string brandCodePrefix;
        private EhlCommsAggregator _ehlCommsAggregator;

        public EnergyProviderAdapter(string providerName, string brandCodePrefix)
        {
            this.providerName = providerName;
            this.brandCodePrefix = brandCodePrefix;
            var ehlHttpClient = new EhlHttpClient();
            _ehlCommsAggregator = new EhlCommsAggregator(ehlHttpClient);
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
                //Quotes = _ehlCommsAggregator.GetPrices(providerEnquiry.ToEhlPriceRequest(),null).Select(el => el.ToEnergyQuote(brandCodePrefix)).ToList()
                Quotes = new List<EnergyQuote>
                    {
                        new EnergyQuote
                        {
                            Brand = brandCodePrefix + "1",
                            Id = providerEnquiry.Enquiry.Id
                        },
                        new EnergyQuote
                        {
                            Brand = brandCodePrefix + "2",
                            Id = providerEnquiry.Enquiry.Id
                        }
                    }

            });
        }
    }
}