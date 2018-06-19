using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Interfaces;
using Energy.ProviderAdapter.ModelConverters;
using Energy.ProviderAdapter.Models;

namespace Energy.ProviderAdapter
{
    //To DO: Change BrandCode logic
    //TO DO: Logging
    //TO DO: Update EnergyEnquery with required fields
    //TO DO; Error handling
    //TO DO: Create http client for adaptor 
    public class EnergyProviderAdapter : IProviderAdapter<EnergyEnquiry, EnergyQuote>
    {
        private readonly string providerName;
        private readonly string brandCodePrefix;
        private IEhlCommsAggregator _ehlCommsAggregator;

        public EnergyProviderAdapter(string providerName, string brandCodePrefix, IEhlCommsAggregator ehlCommsAggregator)
        {
            this.providerName = providerName;
            this.brandCodePrefix = brandCodePrefix;
            this._ehlCommsAggregator = ehlCommsAggregator;
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
                            Note = $"Error from {providerName}, enquiryId: {providerEnquiry.Enquiry.Risk.JourneyId}"
                        }

                    },
                Quotes = _ehlCommsAggregator.GetPrices(providerEnquiry.ToEhlPriceRequest(), providerEnquiry.Environment).Select(el => el.ToEnergyQuote()).ToList().AddFakeBrandCode(brandCodePrefix)

            });
        }
    }
}