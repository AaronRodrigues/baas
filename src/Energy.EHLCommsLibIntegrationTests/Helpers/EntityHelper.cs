
using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models.Prices;
using Energy.EHLCommsLibIntegrationTests.Model;


namespace Energy.EHLCommsLibIntegrationTests.Helpers
{
    public static class EntityHelper
    {
        public static SwitchApplication GetDummySwitchApplication()
        {
            return new SwitchApplication
            {
                CompareWhat = "Both",
                SwitchId = Guid.NewGuid(),
                EhlReference = "somereference",
                ApplicationStatus = "somestatus",
                CreatedTime = DateTime.Now,
                Title = "Mr.",
                FirstName = "test",
                Surname = "test",
                DateOfBirth = DateTime.Today.AddYears(-30),
                ConfirmDate = DateTime.Now,
                EmailAddress = "asd@asd.com",
                SupplyAddress = "10 Some Street, Some Town, Someshire, UK",
                CorrespondenceAddress = "",
                PhoneDayTime = "010101010101",
                ContactAllowedByEmail = false,
                ContactAllowedByPost = false,
                ContactAllowedByTelephone = false,
                ContactAllowedBySms = false,
                NextSteps = "some steps",
                FutureSupplier = "EDF Energy",
                FutureTariff = "Online really big saver",
                TariffDetails = "dunno to be honest",
                HeldApplication = "n/a",
                TariffEndDate = DateTime.Now
            };
        }

        public static ClientEmailRequest GetDummyClientEmailRequest()
        {
            return new ClientEmailRequest
            {
                ResultsKey = new Guid(),
                EmailAddress = "test@test.com",
                FirstName = "test",
                LastName = "tester",
                NumberOfQuotes = "2",
                FilteredPriceResults = GetEmailPriceResults()
            };
        }

        public static JourneyState GetDummyJourneyState()
        {
            return new JourneyState
            {
                JourneyType = "DontHaveMyBill",
                JourneyAction = "YourEnergyPost",
                CompareWhat = "Both",
                ResultId = "someid_101",
                JourneyViewModel = "<EnergyClientViewModel></EnergyClientViewModel>",
                VisitLogId = 132919,
                ApplicationEntryPoint = "http://localhost:8085/energy/DontHaveMyBill",
                SwitchId = Guid.NewGuid(),
                AffclieCode = "CM01SEO",
                Browser = "Chrome",
                BrowserVersion = "23.0",
                BrowserIsMobileDevice = false,
                VisitorId = Guid.NewGuid(),
                VisitorCreationDate = new DateTime(2000, 5, 25),
                LastTouched = new DateTime(2001, 5, 25),
                LastTouchedUtc = new DateTime(2002, 5, 25),
                UrlReferrer = "http://localhost:63973/energy/",
                Message = "test@emailreaction.org",
                Email = "oleg@comparethemarket.com"
            };
        }

        public static GetPricesRequest GenerateValidPricesRequest(StartSwitchResponse response)
        {
            var request = new GetPricesRequest
            {
                CurrentSupplyUrl = response.CurrentSupplyUrl,
                SwitchUrl = response.SwitchStatusUrl,
                CompareType = CompareWhat.Both,
                GasSupplierId = 59,
                GasTariffId = 301,
                GasPaymentMethodId = 2,
                ElectricitySupplierId = 59,
                ElectricityTariffId = 301,
                ElectricityPaymentMethodId = 2,
                ElectricityEco7 = false,
                PercentageNightUsage = 0,
                UseDetailedEstimatorForElectricity = false,
                UseDetailedEstimatorForGas = false
            };

            request.UsageData.GasKwh = 8000;
            request.UsageData.GasUsagePeriod = UsagePeriod.Quarterly;
            request.UsageData.ElectricityKwh = 8000;
            request.UsageData.ElectricityUsagePeriod = UsagePeriod.Quarterly;

            request.SpendData.GasSpendAmount = 500;
            request.SpendData.GasSpendPeriod = UsagePeriod.Annually;
            request.SpendData.ElectricitySpendAmount = 400;
            request.SpendData.ElectricitySpendPeriod = UsagePeriod.Annually;

            request.EstimatorData.HeatingUsage = "1";
            request.EstimatorData.HouseInsulation = "1";
            request.EstimatorData.HouseOccupied = "1";
            request.EstimatorData.HouseType = "4";
            request.EstimatorData.MainCookingSource = "1";
            request.EstimatorData.MainHeatingSource = "0";
            request.EstimatorData.NumberOfBedrooms = "4";
            request.EstimatorData.NumberOfOccupants = "4";

            return request;
        }

        public static ClientPriceResultsMarketingFeedRequest GenerateMarketingRequest()
        {
            return new ClientPriceResultsMarketingFeedRequest
            {
                CompareWhat = "Both",
                Postcode = "PE13DB",
                FirstName = "test",
                LastName = "testsername",
                EmailAddress = "test@test.com",
                EmailOptIn = true,
                PriceResults = new List<MarketingFeedPriceResult>
                                              {
                                                  new MarketingFeedPriceResult
                                                      {
                                                          SupplierName = "test supplier",
                                                          SupplierRating = "5",
                                                          TariffName = "test tariff",
                                                          PaymentMethod = "direct debit",
                                                          AnnualSaving = "1001",
                                                          AnnualSpend = "1500"
                                                      }
                                              }
            };
        }

        public static List<EmailPriceResult> GetEmailPriceResults()
        {
            return new List<EmailPriceResult>
                       {
                           new EmailPriceResult
                               {
                                   TariffType = "Gas",
                                   SupplierName = "EON",
                                   SupplierLogoUrl = "http://test.ctm.com/1.jpg",
                                   ServiceRating = "1",
                                   PaymentType = "DirectDebit",
                                   AnnualSpend = "500.00",
                                   AnnualSaving = "50.00"
                               },
                           new EmailPriceResult
                               {
                                   TariffType = "Gas",
                                   SupplierName = "British Gas",
                                   SupplierLogoUrl = "http://test.ctm.com/2.jpg",
                                   ServiceRating = "5",
                                   PaymentType = "DirectDebit",
                                   AnnualSpend = "800.00",
                                   AnnualSaving = "60.00"
                               },
                          new EmailPriceResult
                               {
                                   TariffType = "Gas",
                                   SupplierName = "EON",
                                   SupplierLogoUrl = "http://test.ctm.com/1.jpg",
                                   ServiceRating = "1",
                                   PaymentType = "DirectDebit",
                                   AnnualSpend = "500.00",
                                   AnnualSaving = "50.00"
                               },
                           new EmailPriceResult
                               {
                                   TariffType = "Gas",
                                   SupplierName = "British Gas",
                                   SupplierLogoUrl = "http://test.ctm.com/2.jpg",
                                   ServiceRating = "5",
                                   PaymentType = "DirectDebit",
                                   AnnualSpend = "800.00",
                                   AnnualSaving = "60.00"
                               },
                           new EmailPriceResult
                               {
                                   TariffType = "Gas",
                                   SupplierName = "EON",
                                   SupplierLogoUrl = "http://test.ctm.com/1.jpg",
                                   ServiceRating = "1",
                                   PaymentType = "DirectDebit",
                                   AnnualSpend = "500.00",
                                   AnnualSaving = "50.00"
                               },
                       };
        }

        public static ApplicationFormData GetStubApplicationFormData()
        {
            return new ApplicationFormDataBuilder()
                .WithHeaderData(Guid.NewGuid(), 59, "EDF", "Standard Price", "Monthly Direct Debit", 100M, 900M, 43M, 1, "Variable")
                .WithDropDownQuestion("customerName", "title", true, new List<OptionItem>
                    {
                        new OptionItem {Id = "Mr", Name = "Mister"},
                        new OptionItem {Id = "Mrs", Name = "Misses"}
                    })
                .WithTextboxQuestion("customerName", "firstName", true)
                .WithTextboxQuestion("customerName", "surname", true)
                .Build();
        }
    }
}
