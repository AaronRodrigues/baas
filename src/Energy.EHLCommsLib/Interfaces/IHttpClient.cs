using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IHttpClient
    {
        T Get<T>(string url);
        T Get<T>(string url, string postData);
    }
}
