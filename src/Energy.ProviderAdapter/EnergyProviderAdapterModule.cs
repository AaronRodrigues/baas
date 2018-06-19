using Autofac;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib;
using Energy.EHLCommsLib.Http;
using Energy.EHLCommsLib.Interfaces;

namespace Energy.ProviderAdapter
{
    //To Do: refactor interfaces and realization to common lib
    public class EnergyProviderAdapterModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterProviderAdapter(builder, Provider.Name, Provider.BrandCodePrefix);
        }

        private static void RegisterProviderAdapter(ContainerBuilder builder, string providerName, string brandCodePrefix)
        {
            builder.RegisterType<EnergyProviderAdapter>()
                .AsImplementedInterfaces()
                .WithMetadata<ProviderAdapterMetadata>(
                    m => m
                        .For(am => am.ProviderName, providerName)
                        .For(am => am.SupportedProduct, Product.Parse("Energy/2.0")))
                .WithMetadata("messaging:component.queueName", $"quoting:Energy:{providerName}:2.0")
                .WithParameter("providerName", providerName)
                .WithParameter("brandCodePrefix", brandCodePrefix);
            builder.RegisterType<EhlHttpClient>().As<IEhlHttpClient>().SingleInstance();
            builder.RegisterType<EhlCommsAggregator>().As<IEhlCommsAggregator>();
            builder.RegisterType<EhlApiCalls>().As<IEhlApiCalls>();
        }
    }
}
