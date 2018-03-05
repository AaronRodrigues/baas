using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.External.Services;
using Energy.EHLCommsLib.Http;
using Energy.EHLCommsLibIntegrationTests.Helpers;
using Energy.EHLCommsLibIntegrationTests.Http;
using Energy.EHLCommsLibIntegrationTests.Model;
using Energy.EHLCommsLibIntegrationTests.Services;
using NUnit.Framework;
using SwitchServiceHelper = Energy.EHLCommsLibIntegrationTests.Services.SwitchServiceHelper;

namespace Energy.EHLCommsLibIntegrationTests
{

    [TestFixture]
    public class EhlCommsAggregatorIntegrationTests
    {
        private const string TestPostcode = "pe26ys";
        private readonly string _apiKey = System.Environment.GetEnvironmentVariable("ehl_api_key");

        private StartSwitchService _startSwitchService;
        private EhlCommsAggregator _ehlCommsAggregator;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();
            var httpClientWrapper = new HttpClientWrapper(httpClient);
            var switchServiceClient = new SwitchServiceClient(httpClientWrapper);
            var switchServiceHelper = new SwitchServiceHelper(switchServiceClient);

            _startSwitchService = new StartSwitchService(switchServiceHelper);
            _ehlCommsAggregator = new EhlCommsAggregator(switchServiceClient);
        }

        [Test]
        public void Should_ReturnPrices_For_KwhUsageWithoutEconomy7()
        {
            var startSwitchResponse = _startSwitchService.StartSwitch(new StartSwitchRequest { Postcode = TestPostcode, ApiKey = _apiKey });
            var pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
            pricesRequest.EnergyJourneyType = EnergyJourneyType.HaveMyBill;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }

        [Test]
        public void Should_ReturnPrices_For_KwhUsageWithEconomy7()
        {
            var startSwitchResponse = _startSwitchService.StartSwitch(new StartSwitchRequest { Postcode = TestPostcode, ApiKey = _apiKey });
            var pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
            pricesRequest.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            pricesRequest.PercentageNightUsage = 0.65m;
            pricesRequest.ElectricityEco7 = true;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }

        [Test]
        public void Should_ReturnPrices_For_SpendUsageOnDontHaveMyBillJourney()
        {
            var startSwitchResponse = _startSwitchService.StartSwitch(new StartSwitchRequest { Postcode = TestPostcode, ApiKey = _apiKey });
            var pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
            pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }

        [Test]
        public void Should_ReturnPrices_For_EstimatorUsageOnDontHaveMyBillJourney()
        {
            var startSwitchResponse = _startSwitchService.StartSwitch(new StartSwitchRequest { Postcode = TestPostcode, ApiKey = _apiKey });
            var pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
            pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;
            pricesRequest.UseDetailedEstimatorForElectricity = true;
            pricesRequest.UseDetailedEstimatorForGas = true;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }

        [Test]
        public void Should_ReturnPrices_For_ElectricityOnlyEstimatorUsageOnDontHaveMyBillJourney()
        {
            var startSwitchResponse = _startSwitchService.StartSwitch(new StartSwitchRequest { Postcode = TestPostcode, ApiKey = _apiKey });
            var pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
            pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;
            pricesRequest.UseDetailedEstimatorForElectricity = true;
            pricesRequest.UseDetailedEstimatorForGas = false;
            pricesRequest.SpendData.GasSpendAmount = 150;
            pricesRequest.SpendData.GasSpendPeriod = UsagePeriod.Monthly;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }

        [Test]
        public void Should_ReturnPrices_For_GasOnlyEstimatorUsageOnDontHaveMyBillJourney()
        {
            var startSwitchResponse = _startSwitchService.StartSwitch(new StartSwitchRequest { Postcode = TestPostcode, ApiKey = _apiKey });
            var pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
            pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;
            pricesRequest.UseDetailedEstimatorForElectricity = false;
            pricesRequest.UseDetailedEstimatorForGas = true;
            pricesRequest.SpendData.ElectricitySpendAmount = 150;
            pricesRequest.SpendData.ElectricitySpendPeriod = UsagePeriod.Monthly;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }
    }
}
