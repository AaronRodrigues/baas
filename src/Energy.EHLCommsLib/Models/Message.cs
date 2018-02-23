using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Models
{
    public class Message
    {
        public MessageCode Code { get; set; }
        public string Item { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
    }
}