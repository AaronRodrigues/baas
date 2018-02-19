using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EHLApiCalls : IEHLApiCalls
    {
        private readonly ISwitchServiceHelper _switchServiceHelper;
        private const string CurrentSupplyRel = "/rels/domestic/current-supply";

        public EHLApiCalls(ISwitchServiceHelper switchServiceHelper)
        {
            _switchServiceHelper = switchServiceHelper;
        }

        public bool CustomerSupplyStageIsSuccessful(GetPricesRequest request, GetPricesResponse response)
        {
            var result = false;
            var currentSupplyTemplate = _switchServiceHelper.GetApiDataTemplate(request.CurrentSupplyUrl, CurrentSupplyRel);
            return result;
        }
    }
}
