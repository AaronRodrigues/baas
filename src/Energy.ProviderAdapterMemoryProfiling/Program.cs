namespace Energy.ProviderAdapterMemoryProfiling
{
    class Program
    {
        static int Main()
        {
            var scenarioRunner = new ScenarioRunner();
            scenarioRunner.Initialize();
            var success = scenarioRunner.Run().Result;
            return success ? 0 : 1;
        }
    }
}
