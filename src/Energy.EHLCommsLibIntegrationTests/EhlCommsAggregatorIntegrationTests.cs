using System.Net.Http;
using System.Threading.Tasks;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Http;
using Energy.EHLCommsLib.Models.Prices;
using Energy.EHLCommsLibIntegrationTests.Helpers;
using Energy.EHLCommsLibIntegrationTests.Services;
using Moq;
using NUnit.Framework;

namespace Energy.EHLCommsLibIntegrationTests
{

    [TestFixture]
    public class EhlCommsAggregatorIntegrationTests
    {


        private StartSwitchHelper _startSwitchHelper;
        private EhlCommsAggregator _ehlCommsAggregator;
        private GetPricesRequest _pricesRequest;

        [SetUp]
        public void Setup()
        {
            var attachments = Mock.Of<IPersistAttachments>();
            var ehlHttpClient = new EhlHttpClient(messageHandler: new WebRequestHandler(), attachmentPersister: attachments);

            _ehlCommsAggregator = new EhlCommsAggregator(new EhlApiCalls(ehlHttpClient));
            _startSwitchHelper = new StartSwitchHelper(ehlHttpClient);

            var startSwitchResponse = _startSwitchHelper.StartSwitch().Result;
            _pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
        }

        [Test]
        public async Task Should_ReturnPrices_For_KwhUsageWithoutEconomy7()
        {
            _pricesRequest.EnergyJourneyType = EnergyJourneyType.HaveMyBill;

            var resultsResponse = await _ehlCommsAggregator.GetPrices(_pricesRequest, "");

            Assert.IsNotEmpty(resultsResponse);
        }

        [Test]
        public async Task Should_ReturnPrices_For_KwhUsageWithEconomy7()
        {
            _pricesRequest.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            _pricesRequest.PercentageNightUsage = 0.65m;
            _pricesRequest.ElectricityEco7 = true;

            var resultsResponse = await _ehlCommsAggregator.GetPrices(_pricesRequest, "");

            Assert.IsNotEmpty(resultsResponse);
        }

        [Test]
        public async Task Should_ReturnPrices_For_SpendUsageOnDontHaveMyBillJourney()
        {
            _pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;

            var resultsResponse = await _ehlCommsAggregator.GetPrices(_pricesRequest, "");

            Assert.IsNotEmpty(resultsResponse);
        }

        [Test]
        public async Task Should_ReturnPrices_For_EstimatorUsageOnDontHaveMyBillJourney()
        {
            _pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;
            _pricesRequest.UseDetailedEstimatorForElectricity = true;
            _pricesRequest.UseDetailedEstimatorForGas = true;

            var resultsResponse = await _ehlCommsAggregator.GetPrices(_pricesRequest, "");

            Assert.IsNotEmpty(resultsResponse);
        }

        [Test]
        public async Task Should_ReturnPrices_For_ElectricityOnlyEstimatorUsageOnDontHaveMyBillJourney()
        {
            var startSwitchResponse = await _startSwitchHelper.StartSwitch();
            var pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);
            pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;
            pricesRequest.UseDetailedEstimatorForElectricity = true;
            pricesRequest.UseDetailedEstimatorForGas = false;
            pricesRequest.SpendData.GasSpendAmount = 150;
            pricesRequest.SpendData.GasSpendPeriod = UsagePeriod.Monthly;

            var resultsResponse = await _ehlCommsAggregator.GetPrices(pricesRequest, "");

            Assert.IsNotEmpty(resultsResponse);
        }

        [Test]
        public async Task Should_ReturnPrices_For_GasOnlyEstimatorUsageOnDontHaveMyBillJourney()
        {
            _pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;
            _pricesRequest.UseDetailedEstimatorForElectricity = false;
            _pricesRequest.UseDetailedEstimatorForGas = true;
            _pricesRequest.SpendData.ElectricitySpendAmount = 150;
            _pricesRequest.SpendData.ElectricitySpendPeriod = UsagePeriod.Monthly;

            var resultsResponse = await _ehlCommsAggregator.GetPrices(_pricesRequest, "");

            Assert.IsNotEmpty(resultsResponse);
        }
    }
}
