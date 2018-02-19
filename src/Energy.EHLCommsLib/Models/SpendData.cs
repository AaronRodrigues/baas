using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models
{
    public class SpendData
    {
        public decimal GasSpendAmount { get; set; }
        public UsagePeriod GasSpendPeriod { get; set; }

        public decimal ElectricitySpendAmount { get; set; }
        public UsagePeriod ElectricitySpendPeriod { get; set; }
    }
}
