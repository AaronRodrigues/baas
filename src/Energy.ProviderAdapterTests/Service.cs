using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
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

        [Test]
        public void Returns_Biscuit_When_A_Biscuit_is_posted()
        {
            var testServer = TestServer.Create(app =>
                app.Run(async context => { context.Response.StatusCode = 200; })
            );
            var httpclient = new HttpClient(testServer.Handler);

            var pJsonContent = "biscuit";
            var httpRequestMessage = new HttpRequestMessage();
            HttpContent httpContent = new StringContent(pJsonContent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;
            var result = httpclient.PostAsync("http://www.biscuitsasaservice.com", httpContent).Result;
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
        private readonly HttpClient _client;

        public BiscuitProvider(HttpClient client)
        {
            _client = client;
        }
        public async Task<PostResponse> CreateBiscuit(Biscuit biscuit)
        {
            //todo: POST to biscuit api with serialised biscuit in the request body
 
            var jsonRequest = JsonConvert.SerializeObject(biscuit);
            var newRequest = new HttpRequestMessage(HttpMethod.Post, "http://www.baas.com/recepticle");
            newRequest.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(newRequest);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.IsSuccessStatusCode ? successfulResponse(response.StatusCode) : unsuccessfulResponse(response.StatusCode);

        }

        private PostResponse successfulResponse(HttpStatusCode responseStatusCode)
        {
            return new PostResponse
            {
                WasSuccessful = true,
                StatusCode = responseStatusCode
            };
        }

        private PostResponse unsuccessfulResponse(HttpStatusCode responseStatusCode)
        {
            return new PostResponse
            {
                WasSuccessful = false,
                StatusCode = responseStatusCode
            };
        }

        public class PostResponse
        {
            public bool WasSuccessful;
            public HttpStatusCode StatusCode;
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
