﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Energy.ProviderAdapterTests.Performance
{
    [TestFixture]
    public class PerformanceTests : OutsideInTestBase
    {
        public void Warmup()
        {
            When_prices_are_requested_for_non_production_environment();
        }

        [Test(Description = "Need to run warmup as it was taking more than 300ms to run for the first time")]
        public void When_performance_test_runs_Then_service_call__should_return_reponse_in_less_than_50ms()
        {
            var totalMs = new List<double>();

            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();
            Warmup();

            for (var i = 0; i < 25; i++)
            {
                When_prices_are_requested_for_non_production_environment();

                TimingCollector.CollectedData.TryGetValue("total", out var totalTimeElapsed);
                totalMs.Add(totalTimeElapsed.TotalMilliseconds);
                TimingCollector.CollectedData.Clear();
            }

            Assert.That(totalMs.All(t => t < 50), Is.True);
        }
    }
}
