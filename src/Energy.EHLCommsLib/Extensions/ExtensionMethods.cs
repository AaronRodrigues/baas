using System;
using System.Text.RegularExpressions;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Extensions
{
    public static class ExtensionMethods
    {
        public static string GetFeatureClassName(this PriceFeatureCategory featureCategory)
        {
            switch (featureCategory)
            {
                case PriceFeatureCategory.CappedOrFixed:
                    return "capped-fixed";
                case PriceFeatureCategory.SpecialOffers:
                    return "special-offers";
                default:
                    return string.Empty;
            }
        }

        public static string EhlSupplyType(this CompareWhat compareWhat)
        {
            switch (compareWhat)
            {
                case CompareWhat.Both:
                    return "4";
                case CompareWhat.Electricity:
                    return "2";
                default:
                    return "1";
            }
        }
    }
}