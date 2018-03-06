using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models;
using Message = Energy.EHLCommsLib.Models.Message;

namespace Energy.EHLCommsLib.Extensions
{
    public static class BaseResponseExtensions
    {
        public static void HydrateSwitchResponseWithErrors(this BaseResponse response, IEnumerable<Error> ehlErrors)
        {
            if (ehlErrors != null)
            {
                foreach (var ehlError in ehlErrors)
                {
                    var code = MapEhlMessageIdToMessageCode(ehlError.Message.Id);
                    response.Messages.Add(new Message
                    {
                        Code = MapEhlMessageIdToMessageCode(ehlError.Message.Id),
                        Item = ehlError.Item,
                        Text = code == MessageCode.ChannelIslandPostcodeEntered ? "Unfortunately this service is not available outside of the UK mainland." : ehlError.Message.Text,
                        Type = MessageType.Error
                    });
                }
            }

            response.ResponseStatusType = DetermineResponseStatusType(response);

            response.Success = false;
        }

        private static MessageCode DetermineResponseStatusType(BaseResponse response)
        {
            if (response.Messages.Any(m => m.Code.Equals(MessageCode.AlreadySwitched)))
                return MessageCode.AlreadySwitched;

            if (response.Messages.Any(m => m.Code.Equals(MessageCode.NegativeElectricityUsage)))
                return MessageCode.NegativeElectricityUsage;

            if (response.Messages.Any(m => m.Code.Equals(MessageCode.NegativeGasUsage)))
                return MessageCode.NegativeGasUsage;

            if (response.Messages.Any(m => m.Code.Equals(MessageCode.ChannelIslandPostcodeEntered)))
                return MessageCode.ChannelIslandPostcodeEntered;

            if (!response.Messages.Any() || response.Messages.Any(m => m.Code.Equals(MessageCode.InternalServerError)))
                return MessageCode.InternalServerError;

            return MessageCode.Unknown;
        }

        private static MessageCode MapEhlMessageIdToMessageCode(string ehlMessageId)
        {
            switch (ehlMessageId)
            {
                case EhlErrorConstants.EhlErrorCodeChannelIsland:
                    return MessageCode.ChannelIslandPostcodeEntered;
                case EhlErrorConstants.EhlErrorOperationNotAvailable:
                    return MessageCode.AlreadySwitched;
                case EhlErrorConstants.EhlErrorGeneric:
                    return MessageCode.InternalServerError;
                case EhlErrorConstants.EhlErrorNegativeElecUsage:
                    return MessageCode.NegativeElectricityUsage;
                case EhlErrorConstants.EhlErrorNegativeGasUsage:
                    return MessageCode.NegativeGasUsage;
                default:
                    return MessageCode.Unknown;
            }
        }
    }
}
