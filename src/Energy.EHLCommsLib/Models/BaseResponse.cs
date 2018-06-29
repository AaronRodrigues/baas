using System.Collections.Generic;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models
{
    public class BaseResponse
    {
        protected BaseResponse()
        {
            Messages = new List<Message>();
            Success = true;
        }

        public List<Message> Messages { get; set; }
        public bool Success { get; set; }
        public MessageCode ResponseStatusType { get; set; }
    }
}