using System;

namespace Energy.EHLCommsLib.Models.Http
{
    public class Token
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}