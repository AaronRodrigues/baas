using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Energy.ProviderAdapterTests;

namespace Energy.ProviderAdapterMemoryProfiling
{
    class ScenarioRunner : OutsideInTestBase
    {

        public async Task WarmUp()
          {
             await Warmup();
          }


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

        public void Run()
        {
            var memoryUsageOverTime = new List<float>();
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();
            for (int i = 0; i < 10; i++)
            {
                When_prices_are_requested_for_production_environment().Wait();
                CalculateMemoryUsedInMb();
                memoryUsageOverTime.Add(CalculateMemoryUsedInMb());
                if (memoryUsageOverTime.Count == 3 || memoryUsageOverTime.Count % 60 == 0)
                    Console.WriteLine($"... currently using {memoryUsageOverTime.Last():F} MB ...");

                Thread.Sleep(100);
            }
        }

        private static async Task<List<float>> Profiler(MemoryProfiler memoryProfiling)
        {
            var memoryUsageOverTime = new List<float>();

            await memoryProfiling.Run(numOfIterations: 150, onIterationComplete: () =>
            {
                memoryUsageOverTime.Add(CalculateMemoryUsedInMb());
                if (memoryUsageOverTime.Count == 3 || memoryUsageOverTime.Count % 60 == 0)
                    Console.WriteLine($"... currently using {memoryUsageOverTime.Last():F} MB ...");
            });
            return memoryUsageOverTime;
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var scenarioRunner = new ScenarioRunner();
            scenarioRunner.Run();
            return 1;
        }
    }
}
