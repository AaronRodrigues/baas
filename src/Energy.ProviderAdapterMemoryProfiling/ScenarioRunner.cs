using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Energy.ProviderAdapterTests;

namespace Energy.ProviderAdapterMemoryProfiling
{
    class ScenarioRunner : OutsideInTestBase
    {
        public void Initialize()
        {
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();
        }

        public async Task<bool> Run()
        {
            var memoryUsageOverTime = new List<float>();

            for (int i = 0; i < 600; i++)
            {
                await When_prices_are_requested_for_production_environment();

                var currentMemoryUsage = CalculateMemoryUsedInMb();
                memoryUsageOverTime.Add(currentMemoryUsage);

                if (i % 60 == 0)
                {
                    Console.WriteLine($"...currently using {currentMemoryUsage} MB");
                }
                
                // Sleep to try and roughly simulate real traffic levels
                Thread.Sleep(100);
            }

            var memoryUsageInMbBefore = memoryUsageOverTime.Skip(2).First();
            var memoryUsageInMbAfter = memoryUsageOverTime.Last();
            var differenceInMb = CalculateDifferenceInMb(memoryUsageOverTime);
            var maxAllowedDifferenceInMb = MaxAllowedDifferenceInMb(memoryUsageInMbBefore);
            return IsMemoryUsageAsExpected(differenceInMb, maxAllowedDifferenceInMb, memoryUsageInMbBefore, memoryUsageInMbAfter);
        }

        private static float CalculateMemoryUsedInMb()
        {
            var memoryPerformanceCounter = new PerformanceCounter(
                "Process",
                "Working Set",
                Process.GetCurrentProcess().ProcessName);

            var memoryUsedInBytes = memoryPerformanceCounter.NextValue();
            var memoryUsedInMb = memoryUsedInBytes / 1024.0f / 1024.0f;
            return memoryUsedInMb;
        }

        private float CalculateDifferenceInMb(IList<float> memoryUsageOverTime)
        {
            var memoryUsageInMbBefore = memoryUsageOverTime.Skip(2).First();
            var memoryUsageInMbAfter = memoryUsageOverTime.Last();
            var differenceInMb = memoryUsageInMbAfter - memoryUsageInMbBefore;
            return differenceInMb;
        }

        private bool IsMemoryUsageAsExpected(float differenceInMb, float maxAllowedDifferenceInMb, float memoryUsageInMbBefore, float memoryUsageInMbAfter)
        {
            if (differenceInMb > maxAllowedDifferenceInMb)
            {
                Console.WriteLine($"*** ERROR: memory usage went from {memoryUsageInMbBefore:F} MB to {memoryUsageInMbAfter:F} MB which is more than our threshold of {maxAllowedDifferenceInMb:F} MB.");
                Console.WriteLine("\tIf this is not expected then check for a memory leak");
                return false;
            }

            Console.WriteLine("Memory usage is as expected.");
            return true;
        }

        private float MaxAllowedDifferenceInMb(float memoryUsageInMbBefore)
        {
            var maxAllowedDifferenceInMb = memoryUsageInMbBefore * 0.05f;
            return maxAllowedDifferenceInMb;
        }
    }
}