using System.Collections.Generic;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEhlCommsAggregator
    {
        Task<List<PriceResult>> GetPrices(GetPricesRequest request, string environment);
    }
}
