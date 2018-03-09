using System.Net;

namespace Energy.EHLCommsLibTests
{
    public class MockEntry
    {
        public MockEntry(string fileName, HttpStatusCode responseStatusCode, WebException exception)
        {
            FileName = fileName;
            ResponseStatusCode = responseStatusCode;
            Exception = exception;
        }

        public string FileName { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public WebException Exception { get; set; }
    }
}
