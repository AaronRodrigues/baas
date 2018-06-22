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
        private static float CalculateMemoryUsedInMb()
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
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly();
            Given_a_valid_enquiry();
            for (int i = 0; i < 1000; i++)
            {
                When_prices_are_requested_for_production_environment().Wait();
                CalculateMemoryUsedInMb();
                Thread.Sleep(100);
            }
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var scenarioRunner = new ScenarioRunner();
            scenarioRunner.Run();
            return CalculateMemoryUsedInMb()  < 1000f ? 1 : 0;
        }
        private static float CalculateMemoryUsedInMb()
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
    }


}
