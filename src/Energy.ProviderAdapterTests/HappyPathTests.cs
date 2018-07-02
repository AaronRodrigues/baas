using System.Linq;
using System.Threading.Tasks;
using CTM.Quoting.Provider;
using NUnit.Framework;
using Moq;


namespace Energy.ProviderAdapterTests
{
    class HappyPathTests : OutsideInTestBase
    {
        public HappyPathTests()
        {
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();
            When_prices_are_requested_for_production_environment().Wait();
        }

        [Test]
        public void Then_Quotes_Are_Returned()
        {
            Assert.Multiple(() =>
            {
                Assert.That(QuotesReturnedByProviderAdapter, Is.Not.Null);
                Assert.That(QuotesReturnedByProviderAdapter.Quotes.First().AnnualSpend, Is.EqualTo(640M));
                Assert.That(QuotesReturnedByProviderAdapter.Quotes.Count(), Is.EqualTo(70));
                Assert.That(QuotesReturnedByProviderAdapter.Quotes.First().SupplierName, Is.EqualTo("EDF Energy"));
                Assert.That(QuotesReturnedByProviderAdapter.Quotes.First().ResultId,
                    Is.EqualTo("tariffSelectionG592694_E592596"));
                AttachmentPersistorMock.Verify(x => x.Save(It.IsAny<Attachment>()), Times.Never());
                Assert.That(RequestCollection.Count, Is.EqualTo(11));
                Assert.That(RequestCollection.First().Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/current-supply"));
                Assert.That(RequestCollection[1].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/current-supply"));
                Assert.That(RequestCollection.First().Method, Is.EqualTo("GET"));
                Assert.That(RequestCollection[1].Method, Is.EqualTo("POST"));
                Assert.That(RequestCollection[2].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/usage"));
                Assert.That(RequestCollection[3].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/usage"));
                Assert.That(RequestCollection[4].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9"));
                Assert.That(RequestCollection[5].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/proratapreference"));
                Assert.That(RequestCollection[6].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/proratapreference"));
                Assert.That(RequestCollection[7].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/preferences"));
                Assert.That(RequestCollection[8].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/preferences"));
                Assert.That(RequestCollection[9].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/future-supply"));
                Assert.That(RequestCollection[10].Path.Value,
                    Is.EqualTo("/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/future-supplies"));
            });
        }
    }
}