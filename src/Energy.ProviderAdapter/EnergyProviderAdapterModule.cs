using Autofac;
using CTM.Quoting.Provider;

namespace Energy.ProviderAdapter
{
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
                        .For(am => am.SupportedProduct, Product.Parse("Energy/1.0")))
                .WithMetadata("messaging:component.queueName", $"quoting:Energy:{providerName}:1.0")
                .WithParameter("providerName", providerName)
                .WithParameter("brandCodePrefix", brandCodePrefix);
        }
    }
}
