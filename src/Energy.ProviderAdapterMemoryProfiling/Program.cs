using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Energy.ProviderAdapterTests;

namespace Energy.ProviderAdapterMemoryProfiling
{
    class ScenarioRunner : OutsideInTestBase
    {
    public static float CalculateMemoryUsedInMb()
        {
            var memoryPerformanceCounter = new PerformanceCounter(
                "Process",
                "Working Set",
                Process.GetCurrentProcess().ProcessName);


            var memoryUsedInBytes = memoryPerformanceCounter.NextValue();
            var memoryUsedInMb = memoryUsedInBytes / 1024.0f / 1024.0f;
            Console.WriteLine(memoryUsedInMb);
            return memoryUsedInMb;
        }

        public int Run()
        {
            var memoryUsageOverTime = new List<float>();
            for (int i = 0; i < 10; i++)
            {
                When_prices_are_requested_for_production_environment().Wait();
                CalculateMemoryUsedInMb();
                memoryUsageOverTime.Add(CalculateMemoryUsedInMb());
                if (memoryUsageOverTime.Count == 3 || memoryUsageOverTime.Count % 60 == 0)
                    Console.WriteLine($"... currently using {memoryUsageOverTime.Last():F} MB ...");

                Thread.Sleep(100);
            }
            var memoryUsageInMbBefore = memoryUsageOverTime.Skip(2).First();
            var memoryUsageInMbAfter = memoryUsageOverTime.Last();
            var differenceInMb = CalculateDifferenceInMb(memoryUsageOverTime);
            var maxAllowedDifferenceInMb = MaxAllowedDifferenceInMb(memoryUsageInMbBefore);
            var memoryComparisonLogger = MemoryComparisonLogger(differenceInMb, maxAllowedDifferenceInMb, memoryUsageInMbBefore, memoryUsageInMbAfter);
            return memoryComparisonLogger;
           
        }

        private int MemoryComparisonLogger(float differenceInMb, float maxAllowedDifferenceInMb, float memoryUsageInMbBefore, float memoryUsageInMbAfter)
        {
            if (differenceInMb > maxAllowedDifferenceInMb)
            {
                Console.WriteLine($"*** ERROR: memory usage went from {memoryUsageInMbBefore:F} MB to {memoryUsageInMbAfter:F} MB which is more than our threshold of {maxAllowedDifferenceInMb:F} MB.");
                Console.WriteLine("\tIf this is not expected then check for a memory leak");
                return 1;
            }

            Console.WriteLine("Memory usage is as expected.");
            return 0;
        }

        private float MaxAllowedDifferenceInMb(float memoryUsageInMbBefore)
        {
            
            var maxAllowedDifferenceInMb = memoryUsageInMbBefore * 0.05f;
            return maxAllowedDifferenceInMb;
        }

        private float CalculateDifferenceInMb(IEnumerable<float> memoryUsageOverTime)
        {
            var memoryUsageInMbBefore = memoryUsageOverTime.Skip(2).First();
            var memoryUsageInMbAfter = memoryUsageOverTime.Last();
            var differenceInMb = memoryUsageInMbAfter - memoryUsageInMbBefore;
            return differenceInMb;
        }

        public void Initialize()
        {
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var scenarioRunner = new ScenarioRunner();
            scenarioRunner.Initialize();
            return scenarioRunner.Run();
        }
    }
}
