using System.Net.Http;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Moq;
using NUnit.Framework;

namespace Energy.EHLCommsLibTests.EhlHttpClient
{
    [TestFixture]
    public class ClientTestBase
    {
        protected const string ProdEnvironment = "prod";
        protected const string NonProdEnvironment = "uat";
        protected const string Url = "http://example.com/api";
        
        protected const string ExpectedResponseBody = "{ \"data-template\": { \"validateAs\": \"test123\" } }";
        protected const string ExpectedErrorResponseBody = "{ \"error\":  \"EHL - Error\" }";

        protected const string ExpectedContentTypeHeader = "application/vnd-fri-domestic-energy+json; version=2.0";
        protected const string ExpectedAcceptHeader = ExpectedContentTypeHeader;

        protected readonly HttpMessageHandler SuccessfulResponseHandler
            = new HttpTestServerBuilder()
                .WithSuccessfulResponse()
                .WithBody(ExpectedResponseBody)
                .Create()
                .Handler;

        protected readonly HttpMessageHandler UnSuccessfulResponseHandler
            = new HttpTestServerBuilder()
                .WithUnsuccessfulResponse()
                .WithBody(ExpectedErrorResponseBody)
                .Create()
                .Handler;

        protected readonly ApiResponse ApiResponseWithData = 
            new ApiResponse { DataTemplate = new DataTemplate { ValidateAs = "test123" } };

        protected IPersistAttachments AttachmentPersister { get; private set; }

        [SetUp]
        public void Setup()
        {
            AttachmentPersister = Mock.Of<IPersistAttachments>();
        }
    }
}
