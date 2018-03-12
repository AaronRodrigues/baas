using CTM.Quoting.Provider;

namespace Energy.ProviderAdapter
{
    public class EnergyQuote : IBrandQuote
    {
        public string Brand { get; set; }
        public decimal Quote { get; set; }
        public string EnquiryId { get; set; }
        public string Note { get; set; }
    }
}
