using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IEHLApiCalls
    {
        bool CustomerSupplyStageIsSuccessful(GetPricesRequest request, GetPricesResponse response);
    }
}
