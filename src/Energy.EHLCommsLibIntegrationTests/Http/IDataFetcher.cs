using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public interface IDataFetcher
    {
        string FetchFrom(string url);
        IDataFetcher UsingTimeOutOf(int timeOutValueInMilliseconds);
    }
}
