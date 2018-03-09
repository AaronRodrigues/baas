using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Exceptions;
using Energy.EHLCommsLib.Models.Prices;
using NUnit.Framework;

namespace Energy.EHLCommsLibTests
{

    [TestFixture]
    public class EhlCommsAggregatorTests
    {
        private const string TestPostcode = "pe26ys";

        private EhlCommsAggregator EhlCommsAggregator { get; set; }
        private MockEhlHttpClient _iHttpClient;


        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var dir = Path.GetDirectoryName(typeof(EhlCommsAggregatorTests).Assembly.Location);
            if (dir != null)
            {
                Environment.CurrentDirectory = dir;
                Directory.SetCurrentDirectory(dir);
            }
            else
                throw new Exception("Path.GetDirectoryName(typeof(EhlCommsAggregatorTests).Assembly.Location) returned null");
        }

        [SetUp]
        public void SetUp()
        {

            _iHttpClient = new MockEhlHttpClient();
            EhlCommsAggregator = new EhlCommsAggregator(_iHttpClient);
        }

        [TearDown]
        public void TearDown()
        {
            //AppSettings.ApplySetting("Feature_TariffCustomFeatureEnabled", "false");
        }

        private void SetupMock()
        {
            _iHttpClient
                .Mock_GetApiResponse( "CurrentSupply-GetResponse", "/current-supply?")
                .Mock_PostSwitchesApiGetResponse("CurrentSupply-PostResponse", "/current-supply?")
                .Mock_GetApiResponse("SwitchStatus-GetResponse", "/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?")
                .Mock_GetApiResponse("ProRata-GetResponse", "/proratapreference?")
                .Mock_PostSwitchesApiGetResponse("ProRata-PostResponse", "/proratapreference?")
                .Mock_GetApiResponse("Usage-GetResponse", "/usage?")
                .Mock_PostSwitchesApiGetResponse("Usage-PostResponse", "/usage?")
                .Mock_GetApiResponse("Preferences-GetResponse", "/preferences?")
                .Mock_PostSwitchesApiGetResponse("Preferences-PostResponse", "/preferences?")
                .Mock_GetApiResponse("FutureSupply-GetResponse", "/future-supply?")
                .Mock_GetApiResponse("FutureSupplies-GetResponse", "/future-supplies?");
        }

        private static GetPricesRequest GetStubPricesRequest()
        {
            return new GetPricesRequest
            {
                CompareType = CompareWhat.Both,
                CurrentSupplyUrl =
                    "http://rest-predeploy.energyhelpline.com/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9/current-supply?b=1+SxdN9QwjA1nP8RoesecNN8ctw",
                SwitchUrl =
                    "http://rest-predeploy.energyhelpline.com/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?b=1+SxdN9QwjA1nP8RoesecNN8ctw",
                Postcode = TestPostcode,
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

        [TestCase(EnergyJourneyType.DontHaveMyBill)]
        public void Should_ReturnPricesAsExpected_UsingDetailedEstimator(EnergyJourneyType journeyType)
        {
            SetupMock();

            var request = GetStubPricesRequest();
            request.EnergyJourneyType = journeyType;
            request.UseDetailedEstimatorForElectricity = true;
            request.UseDetailedEstimatorForGas = true;
            request.EstimatorData.HeatingUsage = "2";
            request.EstimatorData.HouseInsulation = "2";
            request.EstimatorData.HouseOccupied = "0";
            request.EstimatorData.HouseType = "4";
            request.EstimatorData.MainCookingSource = "2";
            request.EstimatorData.MainHeatingSource = "2";
            request.EstimatorData.NumberOfBedrooms = "5";
            request.EstimatorData.NumberOfOccupants = "2";

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.IsTrue(response.Results.Count > 0);
        }

        [Test]
        public void Should_ReturnPrices_UsingSpendDataForDontHaveMyBillJourney()
        {
            SetupMock();

            var request = GetStubPricesRequest();
            request.SpendData.ElectricitySpendAmount = 50;
            request.SpendData.ElectricitySpendPeriod = UsagePeriod.Monthly;
            request.SpendData.GasSpendAmount = 50;
            request.SpendData.GasSpendPeriod = UsagePeriod.Monthly;

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.IsTrue(response.Results.Count > 0);
        }



        [Test]
        public void Should_ReturnPrices_UsingKilowattUsageData()
        {
            _iHttpClient
     .Mock_GetApiResponse("CurrentSupply-GetResponse", "/current-supply?")
     .Mock_PostSwitchesApiGetResponse("CurrentSupply-PostResponse", "/current-supply?")
     .Mock_GetApiResponse( "SwitchStatus-GetResponse", "/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?")
     .Mock_GetApiResponse( "ProRata-GetResponse", "/proratapreference?")
     .Mock_PostSwitchesApiGetResponse( "ProRata-PostResponse", "/proratapreference?")
     .Mock_GetApiResponse( "Usage-GetResponse", "/usage?")
     .Mock_PostSwitchesApiGetResponse( "Usage-PostResponse", "/usage?")
     .Mock_GetApiResponse( "Preferences-GetResponse", "/preferences?")
     .Mock_PostSwitchesApiGetResponse( "Preferences-PostResponse", "/preferences?")
     .Mock_GetApiResponse( "FutureSupply-GetResponse", "/future-supply?")
     .Mock_GetApiResponse( "FutureSupplies-GetResponse", "/future-supplies?");

            var request = GetStubPricesRequest();
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.UsageData.GasKwh = 1000;
            request.UsageData.GasUsagePeriod = UsagePeriod.Monthly;
            request.UsageData.ElectricityKwh = 1000;
            request.UsageData.ElectricityUsagePeriod = UsagePeriod.Monthly;

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.IsTrue(response.Results.Count > 0);
        }

        [Test]
        public void Should_ReturnPrices_UsingSpendDataForHaveMyBillJourney()
        {
            SetupMock();

            var request = GetStubPricesRequest();
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.SpendData.ElectricitySpendAmount = 50;
            request.SpendData.ElectricitySpendPeriod = UsagePeriod.Monthly;
            request.SpendData.GasSpendAmount = 50;
            request.SpendData.GasSpendPeriod = UsagePeriod.Monthly;

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.IsTrue(response.Results.Count > 0);
        }

        [Test]
        public void Should_ReturnPrices_UsingSpendDataForHaveMyBillJourney_AlsoUsingCustomFeatures()
        {
            //AppSettings.ApplySetting("Feature_TariffCustomFeatureEnabled", "true");

            SetupMock();

            var request = GetStubPricesRequest();
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.UsageData.GasKwh = 1000;
            request.UsageData.GasUsagePeriod = UsagePeriod.Monthly;
            request.UsageData.ElectricityKwh = 1000;
            request.UsageData.ElectricityUsagePeriod = UsagePeriod.Monthly;

            const string supplierName = "EDF Energy";
            const string tariffName = "Blue +Price Promise April 2014";
            const string customFeatureText = "Test Custom Feature!!!";
            var customFeatures = new Dictionary<string, string>
            {
                {(supplierName + tariffName).Replace(" ", "").ToLower(), customFeatureText}
            };

            var response = EhlCommsAggregator.GetPrices(request, customFeatures, true);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Results);
            Assert.IsTrue(response.Results.Count > 0);
            Assert.IsTrue(
                response.Results.Any(
                    r =>
                        r.SupplierName.Equals(supplierName) && r.TariffName.Equals(tariffName) &&
                        r.CustomFeatureText.Equals(customFeatureText)));
        }

        [Test]
        public void Should_ReturnAlreadySwitchedErrorResponse_When_AlreadySwitched()
        {

            _iHttpClient.Mock_GetApiResponse( "CurrentSupply-GetResponse-WithError-AlreadySwitched",
                "/current-supply?");

            var request = GetStubPricesRequest();

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Messages.Count > 0);
            Assert.AreEqual(MessageCode.AlreadySwitched, response.ResponseStatusType);
        }

        [Test]
        public void Should_ThrowInvalidSwitchException_When_ApiResponseContainsInternalServerError()
        {
            _iHttpClient.Mock_GetApiResponse(
                "CurrentSupply-GetResponse-WithError-InternalServerError", "/current-supply?",
                HttpStatusCode.InternalServerError,
                new WebException("The remote server returned an error: (500) Internal Server Error."));

            var request = GetStubPricesRequest();

            Assert.Throws<InvalidSwitchException>(() => EhlCommsAggregator.GetPrices(request, null));

        }


        [Test]
        public void Should_ReturnNegativeElecUsageErrorResponse_When_ApiResponseContainsNegativeElecUsage()
        {
            _iHttpClient.Mock_GetApiResponse( "CurrentSupply-GetResponse", "/current-supply?")
                .Mock_PostSwitchesApiGetResponse( "CurrentSupply-PostResponse", "/current-supply?")
                .Mock_GetApiResponse( "SwitchStatus-GetResponse",
                    "/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?")
                .Mock_GetApiResponse( "ProRata-GetResponse", "/proratapreference?")
                .Mock_PostSwitchesApiGetResponse( "ProRata-PostResponse", "/proratapreference?")
                .Mock_GetApiResponse( "Usage-GetResponse", "/usage?")
                .Mock_PostSwitchesApiGetResponse( "Usage-PostResponse-WithError-NegativeElecUsage", "/usage?");

            var request = GetStubPricesRequest();

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Messages.Count > 0);
            Assert.AreEqual(MessageCode.NegativeElectricityUsage, response.ResponseStatusType);
        }

        [Test]
        public void Should_ReturnNegativeElecUsageErrorResponse_When_ApiResponseContainsNegativeGasUsage()
        {
            _iHttpClient.Mock_GetApiResponse( "CurrentSupply-GetResponse", "/current-supply?")
                .Mock_PostSwitchesApiGetResponse( "CurrentSupply-PostResponse", "/current-supply?")
                .Mock_GetApiResponse( "SwitchStatus-GetResponse",
                    "/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?")
                .Mock_GetApiResponse( "ProRata-GetResponse", "/proratapreference?")
                .Mock_PostSwitchesApiGetResponse( "ProRata-PostResponse", "/proratapreference?")
                .Mock_GetApiResponse( "Usage-GetResponse", "/usage?")
                .Mock_PostSwitchesApiGetResponse( "Usage-PostResponse-WithError-NegativeGasUsage", "/usage?");

            var request = GetStubPricesRequest();

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Messages.Count > 0);
            Assert.AreEqual(MessageCode.NegativeGasUsage, response.ResponseStatusType);
        }

        [Test]
        public void Should_ReturnResponseWithErrors_WhenGasSupplierNotEntered()
        {
            _iHttpClient
                .Mock_GetApiResponse( "CurrentSupply-GetResponse", "/current-supply?")
                .Mock_PostSwitchesApiGetResponse( "CurrentSupply-PostResponse-WithErrors", "/current-supply?");

            var request = GetStubPricesRequest();
            request.SpendData.ElectricitySpendAmount = 50;
            request.SpendData.ElectricitySpendPeriod = UsagePeriod.Monthly;
            request.SpendData.GasSpendAmount = 50;
            request.SpendData.GasSpendPeriod = UsagePeriod.Monthly;

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Messages.Count > 0);
        }

        [Test]
        public void Should_ReturnResponseWithProRataSetToTrue_When_ProRataQuestionIsPresented()
        {
            SetupMock();

            var request = GetStubPricesRequest();
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.UsageData.GasKwh = 1000;
            request.UsageData.GasUsagePeriod = UsagePeriod.Monthly;
            request.UsageData.ElectricityKwh = 1000;
            request.UsageData.ElectricityUsagePeriod = UsagePeriod.Monthly;

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.That(response.ProRataCalculationApplied, Is.True);
        }

        [Test]
        public void
            Should_ReturnResponseWithProRataSetToFalse_When_ProRataQuestionIsPresented_ButThePriceRequestModelHasIgnoreProRataComparison
            ()
        {
            SetupMock();

            var request = GetStubPricesRequest();
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.UsageData.GasKwh = 1000;
            request.UsageData.GasUsagePeriod = UsagePeriod.Monthly;
            request.UsageData.ElectricityKwh = 1000;
            request.UsageData.ElectricityUsagePeriod = UsagePeriod.Monthly;
            request.IgnoreProRataComparison = true;
            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.That(response.ProRataCalculationApplied, Is.False);
        }

        [Test]
        public void Should_ReturnResponseWithProRataSetToFalse_When_ProRataQuestionIsNotPresented()
        {
            _iHttpClient
                .Mock_GetApiResponse( "CurrentSupply-GetResponse", "/current-supply?")
                .Mock_PostSwitchesApiGetResponse( "CurrentSupply-PostResponse", "/current-supply?")
                .Mock_GetApiResponse( "SwitchStatus-GetResponse-NoProRataUrl",
                    "/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?")
                .Mock_GetApiResponse( "ProRata-GetResponse", "/proratapreference?")
                .Mock_PostSwitchesApiGetResponse( "ProRata-PostResponse", "/proratapreference?")
                .Mock_GetApiResponse( "Usage-GetResponse", "/usage?")
                .Mock_PostSwitchesApiGetResponse( "Usage-PostResponse", "/usage?")
                .Mock_GetApiResponse( "Preferences-GetResponse", "/preferences?")
                .Mock_PostSwitchesApiGetResponse( "Preferences-PostResponse", "/preferences?")
                .Mock_GetApiResponse( "FutureSupply-GetResponse", "/future-supply?")
                .Mock_GetApiResponse( "FutureSupplies-GetResponse", "/future-supplies?");

            var request = GetStubPricesRequest();
            request.EnergyJourneyType = EnergyJourneyType.HaveMyBill;
            request.UsageData.GasKwh = 1000;
            request.UsageData.GasUsagePeriod = UsagePeriod.Monthly;
            request.UsageData.ElectricityKwh = 1000;
            request.UsageData.ElectricityUsagePeriod = UsagePeriod.Monthly;

            var response = EhlCommsAggregator.GetPrices(request, null);

            Assert.IsNotNull(response);
            Assert.That(response.ProRataCalculationApplied, Is.False);
        }
    }


}

