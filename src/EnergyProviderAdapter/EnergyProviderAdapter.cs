using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTM.Quoting.Provider;

namespace EnergyProviderAdapter
{
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
            throw new NotImplementedException();
        }
    }
}
