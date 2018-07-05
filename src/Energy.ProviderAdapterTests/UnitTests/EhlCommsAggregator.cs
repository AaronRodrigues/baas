using System;
using System.Collections.Generic;
using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Entities;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models.Prices;
using Moq;
using NUnit.Framework;

namespace Energy.ProviderAdapterTests.UnitTests
{
    [TestFixture]
    public class EhlCommsAggregatorTests
    {
        private IEhlApiCalls _ehlApiCalls;

        [OneTimeSetUp]
        public void Setup()
        {
            _ehlApiCalls = Mock.Of<IEhlApiCalls>();
            Mock.Get(_ehlApiCalls).Setup(x => x.GetSupplierEhlApiResponse(It.IsAny<GetPricesRequest>(), It.IsAny<string>())).ReturnsAsync(new EhlApiResponse { NextUrl = "UasageUrl" });
            Mock.Get(_ehlApiCalls).Setup(x => x.GetUsageEhlApiResponse(It.IsAny<GetPricesRequest>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EhlApiResponse { NextUrl = "UasageUrl" });
            Mock.Get(_ehlApiCalls).Setup(x => x.UpdateCurrentSwitchStatus(It.IsAny<GetPricesRequest>(), It.IsAny<string>()));
            Mock.Get(_ehlApiCalls).Setup(x => x.GetPreferenceEhlApiResponse(It.IsAny<GetPricesRequest>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EhlApiResponse { NextUrl = "UasageUrl" });
            Mock.Get(_ehlApiCalls).Setup(x => x.PopulatePricesResponseWithFutureSuppliesFromEhl(It.IsAny<GetPricesRequest>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<PriceResult>());
        }

        [Test]
        public void ShouldGetPricesReturnPrices()
        {
            var aggregator = new EhlCommsAggregator(_ehlApiCalls);
            var result = aggregator.GetPrices(GetPricesRequest(), "test");
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenUsageStageUrlIsEmpty()
        {
            Mock.Get(_ehlApiCalls)
                .Setup(x => x.GetUsageEhlApiResponse(It.IsAny<GetPricesRequest>(), It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(new EhlApiResponse());
            Assert.ThrowsAsync<NullReferenceException>(async () => await new EhlCommsAggregator(_ehlApiCalls).GetPrices(GetPricesRequest(), "test"));
        }

        [Test]
        public void ShouldThrowNullREferenceExceptionWhenSupplyStageUrlIsEmpty()
        {
            Mock.Get(_ehlApiCalls)
                .Setup(x => x.GetSupplierEhlApiResponse(It.IsAny<GetPricesRequest>(), It.IsAny<string>())).ReturnsAsync(new EhlApiResponse());
            Assert.ThrowsAsync<NullReferenceException>(async () => await new EhlCommsAggregator(_ehlApiCalls).GetPrices(GetPricesRequest(), "test"));
        }

        private static GetPricesRequest GetPricesRequest()
        {
            return new GetPricesRequest
            {
                CompareType = CompareWhat.Both,
                CurrentSupplyUrl =
                    "http://rest-predeploy.energyhelpline.com/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/current-supply?b=1+SxdN9QwjA1nP8RoesecNN8ctw",
                SwitchUrl =
                    "http://rest-predeploy.energyhelpline.com/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?b=1+SxdN9QwjA1nP8RoesecNN8ctw",
                Postcode = "PE26YS",
                ElectricitySupplierId = 1,
                EnergyJourneyType = EnergyJourneyType.DontHaveMyBill,
                ElectricityTariffId = 1,
                GasSupplierId = 1,
                GasTariffId = 1,
                PercentageNightUsage = 100M,
                SwitchId = Guid.NewGuid().ToString(),
                ElectricityEco7 = true,
                PrePayment = "false",
            };
        }
    }
}