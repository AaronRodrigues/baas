namespace Energy.ProviderAdapterTests
{
    public class MemoryProfiler : OutsideInTestBase
    {
        protected void Setup()
        {
            Given_the_provider_adapter_is_loaded();
            Given_EHL_API_is_working_correctly(false);
            Given_a_valid_enquiry();
        }
    }
}
