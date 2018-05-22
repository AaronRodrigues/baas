using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.ProviderAdapter.Models
{
    class RequestEstimatorData
    {
        public int NumberOfBedrooms { get; set; }
        public int NumberOfOccupants { get; set; }
        public string MainHeatingSource { get; set; }
        public string HeatingUsage { get; set; }
        public string HouseInsulation { get; set; }
        public string MainCookingSource { get; set; }
        public bool HouseOccupied { get; set; }
        public string HouseType { get; set; }
    }
}
