
using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib
{
    public class EhlCommsAggregator
    {
        private IEHLApiCalls _ehlApiCalls;

        public EhlCommsAggregator(IEHLApiCalls ehlApiCalls)
        {
            _ehlApiCalls = ehlApiCalls;
        }

        public string Test()
        {
            return "Hello World";
        }

        public GetPricesResponse GetPrices(GetPricesRequest request, Dictionary<string, string> customFeatures)
        {
            var response = new GetPricesResponse();
            try
            {
                //Log.Info(string.Format("GetPrices started for JourneyId = {0}, SwitchId = {1}, SwitchUrl = {2}", request.JourneyId, request.SwitchId, request.SwitchUrl));
                var isSupplyStageSuccessful = _ehlApiCalls.CustomerSupplyStageIsSuccessful(request, response);

            }
            catch (Exception)
            {
                
                throw;
            }
           
            return response;
        }
    }
}
