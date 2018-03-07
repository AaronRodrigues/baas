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
                        .For(am => am.SupportedProduct, Product.Parse("Energy/2.0")))
                .WithMetadata("messaging:component.queueName", string.Format("quoting:Energy:{0}:2.0", providerName))
                .WithParameter("providerName", providerName)
                .WithParameter("brandCodePrefix", brandCodePrefix);
        }
    }
}
