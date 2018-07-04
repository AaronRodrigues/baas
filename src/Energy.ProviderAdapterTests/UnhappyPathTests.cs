using System;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Energy.ProviderAdapterTests
{
    [TestFixture]
    public class UnhappyPathTests : OutsideInTestBase
    {
        public class Given_The_Provider_Adpater_Not_Loaded : OutsideInTestBase
        {
            [Test]
            public void Then_Argument_Null_Exception_Should_Be_Thrown()
            {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await When_prices_are_requested_for_production_environment());
            }
        }

        public class Given_EHL_API_Is_Not_Working : OutsideInTestBase
        {
            public Given_EHL_API_Is_Not_Working()
            {
                Given_the_provider_adapter_is_loaded();
                Given_EHL_API_is_not_working();
                When_a_valid_enquiry();
            }

            [Test]
            public void Then_Ehl_should_return_404_and_throw_httprequestexception()
            {
                Assert.ThrowsAsync<HttpRequestException>(async () => await When_prices_are_requested_for_production_environment());
            }
        }

        public class Given_EHL_API_Is_Working_With_Invalid_Future_Supplies : OutsideInTestBase
        {
            public Given_EHL_API_Is_Working_With_Invalid_Future_Supplies()
            {
                Given_the_provider_adapter_is_loaded();
                Given_EHL_API_is_working_correctly_with_invalid_futuresupplies();
            }

            [Test]
            public void Then_Invalid_FutureSupplies_Will_Throw_NullReferenceException_Error()
            {
                When_a_valid_enquiry();
                Assert.ThrowsAsync<NullReferenceException>(async () => await When_prices_are_requested_for_production_environment());
            }
        }

        public class Given_EHL_API_Is_Working : OutsideInTestBase
        {
            public Given_EHL_API_Is_Working()
            {
                Given_the_provider_adapter_is_loaded();
                Given_EHL_API_is_working_correctly();
            }

            [Test]
            public void Then_invalid_enquiry_Will_Return_Error_And_Throws_Exception()
            {
                When_an_invalid_enquiry();
                Assert.ThrowsAsync<HttpRequestException>(async () => await When_prices_are_requested_for_production_environment());
            }

            [Test]
            public void Then_invalid_enquiry_currentsupplyurl_will_throw_InvalidOperationException()
            {
                When_an_invalid_enquiry_with_empty_CurrentSupplyURL();
                Assert.ThrowsAsync<InvalidOperationException>(async () => await When_prices_are_requested_for_production_environment());
            }

            [Test]
            public void Then_Ehl_Return_Invalid_Response_Will_Throw_NullReferenceException()
            {
                When_an_invalid_Response_returned_from_Ehl();
                Assert.ThrowsAsync<NullReferenceException>(async () => await When_prices_are_requested_for_production_environment());
            }
        }

        // Cases for Unhappy Path 
        // - Use the same properties as the error object returned by ehl in the data contract
        // 2. Given object is sent back from GetQuotes doesnt have quotes 
    }
}