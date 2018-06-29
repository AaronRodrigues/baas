namespace Energy.EHLCommsLib.Constants
{
    public static class EhlApiConstants
    {
        public static string UsageTypeByEstimator { get; } = "1";
        public static string UsageTypeByKWhUsage { get; } = "3";
        public static string UsageTypeBySpend { get; } = "4";
        public static string SwitchRel { get; } = "/rels/domestic/switch";
        public static string CurrentSupplyRel { get; } = "/rels/domestic/current-supply";
        public static string ProRataRel { get; } = "/rels/domestic/prorata-preference";
        public static string UsageRel { get; } = "/rels/domestic/usage";
        public static string PreferenceRel { get; } = "/rels/domestic/preferences";
        public static string FutureSupplyRel { get; } = "/rels/domestic/future-supply";
        public static string FutureSuppliesRel { get; } = "/rels/domestic/future-supplies";
        public static string QuoteLinkRel { get; } = "/rels/domestic/quote";
    }
}