using System;
using System.Threading;
using System.Threading.Tasks;
using Energy.ProviderAdapterTests;

namespace Energy.ProviderAdapterMemoryProfiling
{
    class MemoryProfiler: OutsideInTestBase
    {
        public async Task WarmUp()
        {
            await Warmup();
        }

        public async Task Run(int numOfIterations, Action onIterationComplete)
        {
            await Warmup();

            for (var i = 0; i < numOfIterations; ++i)
            {
                await When_prices_are_requested_for_non_production_environment();

                Thread.Sleep(100);

                onIterationComplete();
            }
        }
    }
}