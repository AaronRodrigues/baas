using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models.Prices;
using Energy.EHLCommsLibTests.EhlHttpClient;
using Energy.ProviderAdapter;
using Energy.ProviderAdapter.Models;
using Moq;
using Newtonsoft.Json;

namespace Energy.ProviderAdapterTests
{
    public abstract class OutsideInTestBase
    {
        private readonly ContainerBuilder _containerBuilder = new ContainerBuilder();
        private IContainer _container;
        private EnergyEnquiry _energyEnquiry;

        protected Mock<IPersistAttachments> AttachmentPersistorMock { get; } = new Mock<IPersistAttachments>();
        
        protected void Given_the_provider_adapter_is_loaded()
        {
            _containerBuilder.RegisterModule(new EnergyProviderAdapterModule());

            _containerBuilder.RegisterInstance(AttachmentPersistorMock.Object).As<IPersistAttachments>();
        }

        protected void Given_EHL_API_is_working_correctly()
        {
            var stubbedResponseFilenamesForGetRequests = new Dictionary<string, string>
            {
                { "/current-supply?", "CurrentSupply-GetResponse" },
                { "/domestic/energy/switches/e1b208db-54ab-4cb6-b592-a17f008f6dc9?", "SwitchStatus-GetResponse" },
                { "/proratapreference?", "ProRata-GetResponse" },
                { "/usage?", "Usage-GetResponse" },
                { "/preferences?", "Preferences-GetResponse" },
                { "/future-supply?", "FutureSupply-GetResponse" },
                { "/future-supplies?", "FutureSupplies-GetResponse" }
            };
            var stubbedResponseFilenamesForPostRequests = new Dictionary<string, string>
            {
                { "/current-supply?", "CurrentSupply-PostResponse" },
                { "/proratapreference?", "ProRata-PostResponse" },
                { "/usage?", "Usage-PostResponse" },
                { "/preferences?", "Preferences-PostResponse" },
            };

            var testServer = new HttpTestServerBuilder()
                .WithCallbackForResponse(
                    context =>
                    {
                        var stubbedResponseFilenames = context.Request.Method.ToUpperInvariant() == "GET"
                            ? stubbedResponseFilenamesForGetRequests
                            : stubbedResponseFilenamesForPostRequests;

                        var filenameWithoutExtension = stubbedResponseFilenames
                            .First(x => context.Request.Uri.ToString().Contains(x.Key)).Value;

                        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        var fullFilePath = Path.Combine(basePath, "SwitchApiMessages", $"{filenameWithoutExtension}.json");
                        var fileContents = File.ReadAllText(fullFilePath);
                        context.Response.WriteAsync(fileContents).Wait();
                    })
                .Create();

            _containerBuilder.RegisterInstance(testServer.Handler);
        }

        protected void Given_a_valid_enquiry()
        {
            var stubPriceRequest = new GetPricesRequest
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

            // todo: sort out this work-around (doing this to just get some example data for now)
            // todo: do we need EnergyRisk AND GetPricesRequest as they seem to be very similar?            
            var energyRisk = JsonConvert.DeserializeObject<EnergyRisk>(JsonConvert.SerializeObject(stubPriceRequest));
            _energyEnquiry = new EnergyEnquiry { Risk = energyRisk };
        }

        protected void When_prices_are_requested_for_non_production_environment()
        {
            When_prices_are_requested(environment: "uat");
        }

        protected void When_prices_are_requested_for_production_environment()
        {
            When_prices_are_requested(environment: "prod");
        }

        private void When_prices_are_requested(string environment)
        {
            _container = _containerBuilder.Build();

            var providerAdapter = _container.Resolve<IProviderAdapter<EnergyEnquiry, EnergyQuote>>();
            providerAdapter.GetQuotes(new MakeProviderEnquiry<EnergyEnquiry>
            {
                Environment = environment,
                Enquiry = _energyEnquiry
            });
        }
    }
}