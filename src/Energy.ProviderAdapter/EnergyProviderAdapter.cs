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
        private readonly string _providerName;
        private readonly string _brandCodePrefix;
        private readonly IEhlCommsAggregator _ehlCommsAggregator;

        public EnergyProviderAdapter(string providerName, string brandCodePrefix, IEhlCommsAggregator ehlCommsAggregator)
        {
            _providerName = providerName;
            _brandCodePrefix = brandCodePrefix;
            _ehlCommsAggregator = ehlCommsAggregator;
        }

        public async Task<QuoteResult<EnergyQuote>> GetQuotes(MakeProviderEnquiry<EnergyEnquiry> providerEnquiry)
        {
            return new QuoteResult<EnergyQuote>
            {
                NonQuotes = new List<NonQuote>
                    {
                        new NonQuote
                        {
                            Brand = _brandCodePrefix + "3",
                            Reason = Reason.Error,
                            Note = $"Error from {_providerName}, enquiryId: {providerEnquiry.Enquiry.Risk.JourneyId}"
                        }
                    },
                Quotes = (await _ehlCommsAggregator.GetPrices(providerEnquiry.ToEhlPriceRequest(), providerEnquiry.Environment).ConfigureAwait(false))
                                        .Select(el => el.ToEnergyQuote()).ToList().AddFakeBrandCode(_brandCodePrefix)
            };
        }
    }
}