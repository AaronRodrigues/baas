using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Extensions;

namespace Energy.EHLCommsLib.Models.Prices
{
    public class PriceFeature
    {
        public PriceFeature()
        {
        }

        public PriceFeature(PriceFeatureCategory category, string description)
        {
            Category = category;
            Description = description;
            ClassName = Category.GetFeatureClassName();
        }

        public PriceFeatureCategory Category { get; set; }
        public string Description { get; set; }
        public string ClassName { get; set; }
    }
}