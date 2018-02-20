using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.SwitchApiResponse
{
    [DataContract]
    public class ResultPreferences
    {
        [DataMember(Name = "resultOrder")]
        public NameIdPair ResultsOrder { get; set; }

        [DataMember(Name = "showPaymentTypes")]
        public List<NameIdPair> ShowPaymentTypes { get; set; }

        [DataMember(Name = "tariffFilterOptions")]
        public NameIdPair TariffFilterOptions { get; set; }
    }
}
