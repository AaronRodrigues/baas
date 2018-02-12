using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTM.Quoting.Provider;

namespace EnergyProviderAdapter
{
    public class EnergyQuote : IBrandQuote
    {
        public string Brand { get; set; }
    }
}
