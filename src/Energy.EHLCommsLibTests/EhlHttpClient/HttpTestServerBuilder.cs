using System;
using System.IO;
using Microsoft.Owin;
using Microsoft.Owin.Testing;
using Owin;

namespace Energy.EHLCommsLibTests.EhlHttpClient
{
    public class HttpTestServerBuilder
    {
        private int _statusCode;
        private string _responseBody = "{}";
        private Action<string, IOwinRequest> _requestCallback = (_, __) => {};
        private Action<IOwinContext> _responseCallback;

        public HttpTestServerBuilder WithSuccessfulResponse()
        {
            _statusCode = 200;
            return this;
        }

        public HttpTestServerBuilder WithUnsuccessfulResponse()
        {
            _statusCode = 400;
            return this;
        }

        public HttpTestServerBuilder WithBody(string responseBody)
        {
            _responseBody = responseBody;
            return this;
        }

        public HttpTestServerBuilder WithCallbackForResponse(Action<IOwinContext> responseCallback)
        {
            _responseCallback = responseCallback;
            return this;
        }

        public HttpTestServerBuilder WithCallbackForRequest(Action<string, IOwinRequest> requestCallback)
        {
            _requestCallback = requestCallback;
            return this;
        }
        
        public TestServer Create()
        {
            var testServer = TestServer.Create(app =>
                app.Run(async context =>
                {
                    var request = context.Request;
                    var requestBody = new StreamReader(request.Body).ReadToEnd();
                    _requestCallback(requestBody, context.Request);

                    if (_responseCallback != null)
                    {
                        _responseCallback(context);
                    }
                    else
                    {
                        context.Response.StatusCode = _statusCode;
                        await context.Response.WriteAsync(_responseBody);
                    }
                }));
            return testServer;
        }
    }
}