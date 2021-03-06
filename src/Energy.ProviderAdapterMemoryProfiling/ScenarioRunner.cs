﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Energy.ProviderAdapterTests;

namespace Energy.ProviderAdapterMemoryProfiling
{
    class ScenarioRunner : MemoryProfiler
    {
        public void Initialize()
        {
            Setup();
        }

        public async Task<bool> Run()
        {
            var memoryUsageOverTime = new List<float>();
            var numberOfIterations = 600;
            for (int i = 0; i < numberOfIterations; i++)
            {
                await When_prices_are_requested_for_production_environment().ConfigureAwait(false);

                var currentMemoryUsage = CalculateMemoryUsedInMb();
                memoryUsageOverTime.Add(currentMemoryUsage);

                if (i % 60 == 0)
                {
                    Console.WriteLine($"...currently using {currentMemoryUsage} MB");
                }
                
                // Sleep to try and roughly simulate real traffic levels
                Thread.Sleep(100);
            }

            var memoryUsageInMbBefore = memoryUsageOverTime.Skip(10).First();
            var memoryUsageInMbAfter = memoryUsageOverTime.Last();
            var differenceInMb = CalculateDifferenceInMb(memoryUsageInMbBefore, memoryUsageInMbAfter);
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

        private float CalculateDifferenceInMb(float memoryUsageInMbBefore, float memoryUsageInMbAfter)
        {
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
            var maxAllowedDifferenceInMb = memoryUsageInMbBefore * 0.1f;
            return maxAllowedDifferenceInMb;
        }
    }
}