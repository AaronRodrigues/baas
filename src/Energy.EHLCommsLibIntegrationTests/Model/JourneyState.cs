using System;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class JourneyState
    {
        public JourneyState()
        {
            CreateTime = DateTime.Now;
        }
        public int Id { get; set; }
        public string JourneyType { get; set; }
        public string JourneyAction { get; set; }
        public string ApplicationEntryPoint { get; set; }
        public Guid? SwitchId { get; set; }
        public string CompareWhat { get; set; }
        public string ResultId { get; set; }
        public string Message { get; set; }
        public string JourneyViewModel{ get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public bool BrowserIsMobileDevice { get; set; }
        public string AffclieCode { get; set; }
        public string UrlReferrer { get; set; }

        // MVT cookie
        public Guid VisitorId { get; set; }
        public DateTime? VisitorCreationDate { get; set; }
        
        // MVT Session cookie
        public long  VisitLogId { get; set; }
        public DateTime? LastTouched { get; set; }
        public DateTime? LastTouchedUtc { get; set; }
        public DateTime CreateTime { get; set; }

        public string Email { get; set; }
    }
} 
