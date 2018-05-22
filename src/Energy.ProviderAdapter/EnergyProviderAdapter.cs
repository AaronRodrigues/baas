using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Http;
using Energy.ProviderAdapter.ModelConverters;
using Energy.ProviderAdapter.Models;

namespace Energy.ProviderAdapter
{
    //TO DO: Logging
    //TO DO: Update EnergyEnquery with required fields
    //TO DO; Error handling
    //TO DO: Create http client for adaptor 
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
            new QuoteResult<EnergyQuote>{
                Quotes = _ehlCommsAggregator.GetPrices(providerEnquiry.ToEhlPriceRequest()).Select(el => el.ToEnergyQuote(brandCodePrefix)).ToList()
            });
        }
    }
}