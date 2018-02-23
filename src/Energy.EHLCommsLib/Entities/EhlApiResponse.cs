using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Entities
{
    public class EhlApiResponse
    {
        public string ConcatenatedErrorString { get; set; }
        public bool ApiCallWasSuccessful { get; set; }
        public string ApiStage { get; set; }
        public string NextUrl { get; set; }
    }
}
