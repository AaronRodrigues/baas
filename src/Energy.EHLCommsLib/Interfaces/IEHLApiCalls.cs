using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEHLApiCalls
    {
        EhlApiResponse GetCustomerSupplyStageResult(GetPricesRequest request, GetPricesResponse response);
        EhlApiResponse GetUsageStageResult(GetPricesRequest request, GetPricesResponse response, string usageUrl);
    }
}
