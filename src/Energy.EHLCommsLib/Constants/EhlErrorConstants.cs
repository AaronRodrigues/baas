namespace Energy.EHLCommsLib.Constants
{
    public static class EhlErrorConstants
    {
        public static string EhlErrorGeneric { get; } = "/errors/error";
        public static string EhlErrorCodeChannelIsland { get; } = "/errors/domestic/channel-islands-not-supported";
        public static string EhlErrorOperationNotAvailable { get; } = "/errors/operation-not-available";
        public static string EhlErrorNegativeElecUsage { get; } = "/errors/domestic/negative-elec-usage";
        public static string EhlErrorNegativeGasUsage { get; } = "/errors/domestic/negative-gas-usage";
        public static string EhlErrorInvalidPostcode { get; } = "/errors/domestic/invalid-uk-postcode";

        // We are purposely not including the generic error in this collection
        public static readonly string[] ExpectedErrors =
        {
            EhlErrorCodeChannelIsland,
            EhlErrorOperationNotAvailable,
            EhlErrorNegativeElecUsage,
            EhlErrorNegativeGasUsage,
            EhlErrorInvalidPostcode
        };
    }
}