using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Energy.ProviderAdapterTests;
using NUnit.Framework;

namespace Energy.ProviderAdapterPerformanceTests
{
    [TestFixture, Category("performance")]
    public class PerformanceTests : OutsideInTestBase
    {
        [Test(Description = "Need to run warmup as it was taking more than 300ms to run for the first time")]
        public async Task When_performance_test_runs_Then_service_call_should_return_reponse_in_less_than_50ms()
        {
            var totalMs = new List<double>();

            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            When_a_valid_enquiry();
            await Warmup();

            for (var i = 0; i < 25; i++)
            {
                await When_prices_are_requested_for_non_production_environment();

                TimingCollector.CollectedData.TryGetValue("total", out var totalTimeElapsed);
                totalMs.Add(totalTimeElapsed.TotalMilliseconds);
                TimingCollector.CollectedData.Clear();
            }

            Assert.That(totalMs.All(t => t < 50), Is.True);
        }
    }
}
