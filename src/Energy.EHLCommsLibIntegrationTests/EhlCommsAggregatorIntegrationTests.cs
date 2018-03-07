using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Http;
using Energy.EHLCommsLib.Models.Prices;
using Energy.EHLCommsLibIntegrationTests.Helpers;
using Energy.EHLCommsLibIntegrationTests.Services;
using NUnit.Framework;

namespace Energy.EHLCommsLibIntegrationTests
{

    [TestFixture]
    public class EhlCommsAggregatorIntegrationTests
    {


        private StartSwitchHelper _startSwitchHelper;
        private EhlCommsAggregator _ehlCommsAggregator;
        private GetPricesRequest pricesRequest;

        [SetUp]
        public void Setup()
        {
            var ehlHttpClient = new EhlHttpClient();

            _ehlCommsAggregator = new EhlCommsAggregator(ehlHttpClient);

            var startSwitchResponse = _startSwitchHelper.StartSwitch();


            pricesRequest = EntityHelper.GenerateValidPricesRequest(startSwitchResponse);


        }

        [Test]
        public void Should_ReturnPrices_For_KwhUsageWithoutEconomy7()
        {
            pricesRequest.EnergyJourneyType = EnergyJourneyType.HaveMyBill;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }

        [Test]
        public void Should_ReturnPrices_For_KwhUsageWithEconomy7()
        {
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
            pricesRequest.EnergyJourneyType = EnergyJourneyType.DontHaveMyBill;

            var resultsResponse = _ehlCommsAggregator.GetPrices(pricesRequest, null);

            Assert.AreEqual(true, resultsResponse.Success);
            Assert.AreNotEqual(0, resultsResponse.Results);
        }

        [Test]
        public void Should_ReturnPrices_For_EstimatorUsageOnDontHaveMyBillJourney()
        {
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
            var startSwitchResponse = _startSwitchHelper.StartSwitch();
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
