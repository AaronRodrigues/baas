using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Models
{
    public class EstimatorData
    {
        public string NumberOfBedrooms { get; set; }
        public string NumberOfOccupants { get; set; }
        public string MainHeatingSource { get; set; }
        public string HeatingUsage { get; set; }
        public string HouseInsulation { get; set; }
        public string MainCookingSource { get; set; }
        public string HouseOccupied { get; set; }
        public string HouseType { get; set; }
    }
}
