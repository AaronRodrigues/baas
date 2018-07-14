using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;

namespace Energy.ProviderAdapterTests
{
    class Service
    {
        [Test]
        public void Returns_HttpStatusCodeOK_When_GET_Request_Is_Made()
        {
            var testServer = TestServer.Create(app =>
                app.Run(async context => { context.Response.StatusCode = 200; })
            );
            var httpclient = new HttpClient(testServer.Handler);
            var result = httpclient.GetAsync("http://www.biscuitsasaservice.com").Result;
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }



    }

    public class Biscuit
    {
        public int Firmness;
        public string Name;
        public string Id;
    }
    public class BiscuitProvider
    {
        public void CreateBiscuit(Biscuit biscuit)
        {
            // todo: POST to biscuit api with serialised biscuit in the request body

        }

        public Biscuit FindBiscuitById(string id)
        {
            // todo: ==do a GET on the biscuit API and deserialize the json in the response body into a Biscuit
            return null;
        }
    }

    /*
     * given biscuit exists in API
     * when getting biscuit
     * biscuit is returned
     *
     * given biscuit does not exist in api
     * when getting biscuit
     * an exception is thrown
     *
     *
     */
}
