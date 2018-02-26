using System.IO;
using System.Net;
using System.Text;
using CTM.Energy.Common.Interfaces;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public class HttpResponse : IResponse
    {
        private readonly HttpWebResponse _rawWebResponse;

        public HttpResponse(HttpWebResponse webResponse) { _rawWebResponse = webResponse; }

        public HttpStatusCode StatusCode { get { return _rawWebResponse.StatusCode; } }

        public string StatusDescription { get { return _rawWebResponse.StatusDescription; } }

        public string BodyAsString()
        {
            using (var reader = new StreamReader(_rawWebResponse.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public byte[] BodyAsBinary()
        {
            using (var memoryStream = new MemoryStream())
            {
                _rawWebResponse.GetResponseStream().CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
