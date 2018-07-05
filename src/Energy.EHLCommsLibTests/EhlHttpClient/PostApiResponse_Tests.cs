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
    public class PostApiResponse_Tests : ClientTestBase
    {
        [Test]
        public async Task A_POST_request_is_made_using_the_given_parameters()
        {
            IOwinRequest requestMade = null;
            string requestBody = string.Empty;

            var testServer = new HttpTestServerBuilder()
                .WithSuccessfulResponse()
                .WithCallbackForRequest((body, req) =>
                {
                    requestBody = body; 
                    requestMade = req;
                }).Create();

            var client = new EHLCommsLib.Http.EhlHttpClient(testServer.Handler, AttachmentPersister);

            await client.PostApiGetResponse(Url, ApiResponseWithData, NonProdEnvironment);

            Assert.Multiple(() =>
            {
                Assert.That(requestMade, Is.Not.Null);
                Assert.That(requestMade.Method, Is.EqualTo("POST"));
                Assert.That(requestMade.Uri.ToString(), Is.EqualTo(Url));
                Assert.That(requestBody, Is.EqualTo("{\"data-template\":{\"validateAs\":\"test123\"}}"));
            });
            Assert.That(requestMade.Accept, Is.EqualTo(ExpectedAcceptHeader));
            Assert.That(requestMade.ContentType, Is.EqualTo(ExpectedContentTypeHeader));
        }

        [Test]
        public async Task When_the_environment_is_not_prod_The_request_is_saved_as_attachment()
        {
            var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);
            await client.PostApiGetResponse(Url, ApiResponseWithData, NonProdEnvironment);

            Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.AtLeastOnce);
            Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.Is<Attachment>
                (att => att.Content.Contains(ExpectedResponseBody)
                        && string.Equals(att.MediaType, "application/json",
                            StringComparison.InvariantCultureIgnoreCase))),
                Times.Once());
        }

        [Test]
        public async Task When_the_environment_is_prod_The_request_is_not_saved_as_attachment()
        {
            var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);
            await client.PostApiGetResponse(Url, ApiResponseWithData, ProdEnvironment);

            Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.Never);            
        }

        public class Given_the_server_returns_a_successful_response : ClientTestBase
        {
            [Test]
            public async Task Then_the_response_is_deserialised_into_a_model()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);

                var result = await client.PostApiGetResponse(Url, new ApiResponse(), NonProdEnvironment);

                Assert.That(result.DataTemplate.ValidateAs, Is.EqualTo("test123"));
            }
            
            [Test]
            public async Task When_the_environment_is_not_prod_Then_the_response_is_saved_as_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);
                await client.PostApiGetResponse(Url, ApiResponseWithData, NonProdEnvironment);

                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.AtLeastOnce);
                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.Is<Attachment>
                    (att => att.Content.Contains(ExpectedResponseBody)
                            && string.Equals(att.MediaType, "application/json",
                                StringComparison.InvariantCultureIgnoreCase))),
                    Times.Once());
            }

            [Test]
            public async Task When_the_environment_is_prod_Then_the_response_is_not_saved_as_an_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(SuccessfulResponseHandler, AttachmentPersister);
                await client.PostApiGetResponse(Url, ApiResponseWithData, ProdEnvironment);

                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.Never);
            }
        }

        public class Given_the_server_returns_a_unsuccessful_response : ClientTestBase
        {
            [Test]
            public void Then_an_exception_is_thrown()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(UnSuccessfulResponseHandler, AttachmentPersister);

                Assert.ThrowsAsync<HttpRequestException>(() => client.PostApiGetResponse(Url, new ApiResponse(), NonProdEnvironment));
            }

            [Test]
            public void When_environment_is_not_prod_Then_the_response_is_saved_as_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(UnSuccessfulResponseHandler, AttachmentPersister);

                Assert.ThrowsAsync<HttpRequestException>(() => client.PostApiGetResponse(Url, new ApiResponse(), NonProdEnvironment));

                Mock.Get(AttachmentPersister)
                    .Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.AtLeastOnce);
                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.Is<Attachment>
                    (att => att.Content.Contains(ExpectedErrorResponseBody)
                            && string.Equals(att.MediaType, "application/json",
                                StringComparison.InvariantCultureIgnoreCase))),
                    Times.Once());
            }

            [Test]
            public void When_environment_is_prod_Then_the_response_is_not_saved_as_attachment()
            {
                var client = new EHLCommsLib.Http.EhlHttpClient(UnSuccessfulResponseHandler, AttachmentPersister);

                Assert.ThrowsAsync<HttpRequestException>(() => client.PostApiGetResponse(Url, new ApiResponse(), ProdEnvironment));

                Mock.Get(AttachmentPersister).Verify(attachments => attachments.Save(It.IsAny<Attachment>()), Times.Never);
            }
        }
    }
}