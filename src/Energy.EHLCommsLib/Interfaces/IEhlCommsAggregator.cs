using System.Collections.Generic;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEhlCommsAggregator
    {
        List<PriceResult> GetPrices(GetPricesRequest request, string environment);
    }
}
