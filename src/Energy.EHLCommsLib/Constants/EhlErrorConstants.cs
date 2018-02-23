namespace Energy.EHLCommsLib.Constants
{
    public class EhlErrorConstants
    {
        public const string EhlErrorGeneric = "/errors/error";
        public const string EhlErrorCodeChannelIsland = "/errors/domestic/channel-islands-not-supported";
        public const string EhlErrorOperationNotAvailable = "/errors/operation-not-available";
        public const string EhlErrorNegativeElecUsage = "/errors/domestic/negative-elec-usage";
        public const string EhlErrorNegativeGasUsage = "/errors/domestic/negative-gas-usage";
        public const string EhlErrorInvalidPostcode = "/errors/domestic/invalid-uk-postcode";
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