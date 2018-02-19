using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models
{
    public abstract class BaseRequest
    {
        public string Postcode { get; set; }
        public string SwitchId { get; set; }
        public EnergyJourneyType EnergyJourneyType { get; set; }
    }
}
