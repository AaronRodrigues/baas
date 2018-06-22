using CTM.Quoting.Provider;
using Moq;
using NUnit.Framework;

namespace Energy.ProviderAdapterTests
{
    public class Attachments_are_saved_in_test_environment : OutsideInTestBase
    {
        public Attachments_are_saved_in_test_environment()
        {
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();

            When_prices_are_requested_for_non_production_environment().Wait();
        }

        [Test]
        public void Then_attachments_are_saved()
        {
            AttachmentPersistorMock.Verify(x => x.Save(It.IsAny<Attachment>()), Times.AtLeastOnce());
        }
    }

    public class Attachments_are_not_saved_in_production_environment : OutsideInTestBase
    {
        public Attachments_are_not_saved_in_production_environment()
        {
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();

            When_prices_are_requested_for_production_environment().Wait();
        }

        [Test]
        public void Then_no_attachments_are_saved()
        {
            AttachmentPersistorMock.Verify(x => x.Save(It.IsAny<Attachment>()), Times.Never());
        }
    }
}