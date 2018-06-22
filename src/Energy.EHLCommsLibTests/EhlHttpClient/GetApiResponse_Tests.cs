using System;
using System.Net.Http;
using System.Threading.Tasks;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Contracts.Responses;
using Microsoft.Owin;
using Moq;
using NUnit.Framework;

namespace Energy.EHLCommsLibTests.EhlHttpClient
{
    public class GetApiResponse_Tests : ClientTestBase
    {
        [Test]
        public async Task A_GET_request_is_made_to_the_url_provided()
        {
            IOwinRequest requestMade = null;
            var testServer = new HttpTestServerBuilder()
                .WithSuccessfulResponse()
                .WithCallbackForRequest((_,req) => requestMade = req)
                .Create();

            var client = new EHLCommsLib.Http.EhlHttpClient(testServer.Handler, AttachmentPersister);

            await client.GetApiResponse<ApiResponse>(Url, NonProdEnvironment);

            Assert.That(requestMade, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(requestMade.Method, Is.EqualTo("GET"));
                Assert.That(requestMade.Uri.ToString(), Is.EqualTo(Url));

                Assert.That(requestMade.Accept, Is.EqualTo(ExpectedAcceptHeader));
            });
        }

        public class Given_the_server_returns_a_successful_response : ClientTestBase
        {
            [Test]
            public async Task Then_the_response_is_deserialised_into_a_model()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);

                var result = await client.GetApiResponse<ApiResponse>(Url, NonProdEnvironment);

                Assert.That(result.DataTemplate.ValidateAs, Is.EqualTo("test123"));
            }

            [Test]
            public async Task When_environment_is_not_prod_Then_the_response_is_saved_as_an_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);

                await client.GetApiResponse<ApiResponse>(Url, NonProdEnvironment);

                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.Once());
                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.Is<Attachment>
                    (att => att.Content.Equals(ExpectedResponseBody, StringComparison.InvariantCultureIgnoreCase)
                            && string.Equals(att.MediaType, "application/json",
                                StringComparison.InvariantCultureIgnoreCase))),
                    Times.Once());
            }

            [Test]
            public async Task When_environment_is_prod_Then_the_response_is_not_saved_as_an_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);

                await client.GetApiResponse<ApiResponse>(Url, ProdEnvironment);

                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.Never);
            }
    
        }

        public class Given_the_server_returns_a_unsuccessful_response : ClientTestBase
        {
            [Test]
            public void Then_an_exception_is_thrown()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(UnSuccessfulResponseHandler, AttachmentPersister);

                Assert.ThrowsAsync<HttpRequestException>(() => client.GetApiResponse<ApiResponse>(Url, NonProdEnvironment));
            }

            [Test]
            public void When_environment_is_not_prod_Then_the_response_is_saved_as_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(UnSuccessfulResponseHandler, AttachmentPersister);

                Assert.ThrowsAsync<HttpRequestException>(() => client.GetApiResponse<ApiResponse>(Url, NonProdEnvironment));

                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.Once());
                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.Is<Attachment>
                    (att => att.Content.Equals(ExpectedErrorResponseBody, StringComparison.InvariantCultureIgnoreCase)
                            && string.Equals(att.MediaType, "application/json",
                                StringComparison.InvariantCultureIgnoreCase))),
                    Times.Once());
            }

            [Test]
            public void When_environment_is_prod_Then_the_response_is_not_saved_as_an_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(UnSuccessfulResponseHandler, AttachmentPersister);

                Assert.ThrowsAsync<HttpRequestException>(() => client.GetApiResponse<ApiResponse>(Url, ProdEnvironment));

                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.Never);
            }
        }
    }
}