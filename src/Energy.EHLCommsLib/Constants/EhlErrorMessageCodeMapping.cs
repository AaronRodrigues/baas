using System.Collections.Generic;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Constants
{
    public static class EhlErrorMessageCodeMapping
    {
        public static IReadOnlyDictionary<string, MessageCode> EhlErrorMessageCode { get; } = new Dictionary<string, MessageCode>
        {
            { EhlErrorConstants.EhlErrorCodeChannelIsland, MessageCode.ChannelIslandPostcodeEntered },
            { EhlErrorConstants.EhlErrorOperationNotAvailable, MessageCode.AlreadySwitched },
            { EhlErrorConstants.EhlErrorGeneric, MessageCode.InternalServerError },
            { EhlErrorConstants.EhlErrorNegativeElecUsage, MessageCode.NegativeElectricityUsage },
            { EhlErrorConstants.EhlErrorNegativeGasUsage, MessageCode.NegativeGasUsage }
        };
    }
}