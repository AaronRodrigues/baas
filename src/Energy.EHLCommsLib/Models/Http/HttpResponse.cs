using System.IO;
using System.Net;
using System.Text;
using Energy.EHLCommsLib.Interfaces.Http;

namespace Energy.EHLCommsLib.Models.Http
{
    public class HttpResponse : IResponse
    {
        private readonly HttpWebResponse _rawWebResponse;

        public HttpResponse(HttpWebResponse webResponse) { _rawWebResponse = webResponse; }

        public HttpStatusCode StatusCode => _rawWebResponse.StatusCode;

        public string StatusDescription => _rawWebResponse.StatusDescription;

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
                var responseStream = _rawWebResponse.GetResponseStream();
                responseStream?.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
