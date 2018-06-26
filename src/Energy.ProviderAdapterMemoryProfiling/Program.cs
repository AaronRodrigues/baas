using System.Threading.Tasks;

namespace Energy.ProviderAdapterMemoryProfiling
{
    class Program
    {
        static async Task<int> Main()
        {
            var scenarioRunner = new ScenarioRunner();
            scenarioRunner.Initialize();
            var success = await scenarioRunner.Run();
            return success ? 0 : 1;
        }
    }
}
