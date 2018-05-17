using System.Linq;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Models.Prices;
using Energy.ProviderAdapter.Models;

namespace Energy.ProviderAdapter.ModelConverters
{
    public static class EhlModelsConverterHelper
    {
        public static EnergyQuote ToEnergyQuote(this PriceResult priceResult, string brandCode)
        {
            var result = new EnergyQuote
            {
                Brand = brandCode,
                KeyFeatures = priceResult.KeyFeatures.Select(
                    x =>
                        new Models.PriceFeature
                        {
                            Category = x.Category.ToString(),
                            ClassName = x.ClassName,
                            Description = x.Description
                        }).ToList(),
                Id = priceResult.Id,
                ResultId = priceResult.ResultId,
                SupplierId = priceResult.SupplierId,
                SupplierName = priceResult.SupplierName,
                SupplierRating = priceResult.SupplierRating,
                SupplierCss = priceResult.SupplierCss,
                TariffId = priceResult.TariffId,
                TariffName = priceResult.TariffName,
                TariffDetailsUrl = priceResult.TariffDetailsUrl,
                HasTariffCustomFeature = priceResult.HasTariffCustomFeature,
                CustomFeatureText = priceResult.CustomFeatureText,
                PaymentMethod = (int)priceResult.PaymentMethod,
                PaymentMethodId = priceResult.PaymentMethodId,
                CanApply = priceResult.CanApply,
                AnnualSavings = priceResult.AnnualSavings,
                AnnualSpend = priceResult.AnnualSpend,
                ElectricityAnnualSavings = priceResult.ElectricityAnnualSavings,
                ElectricityAnnualSpend = priceResult.ElectricityAnnualSpend,
                GasAnnualSavings = priceResult.GasAnnualSavings,
                GasAnnualSpend = priceResult.GasAnnualSpend,
                RenewableFuelPercentage = priceResult.RenewableFuelPercentage,
                PaperLessBilling = priceResult.PaperLessBilling,
                PaperBilling = priceResult.PaperBilling,
                NoStandingCharges = priceResult.NoStandingCharges,
                CappedOrFixed = priceResult.CappedOrFixed,
                Green = priceResult.Green,
                AccurateMonthlyBilling = priceResult.AccurateMonthlyBilling,
                StayWarm = priceResult.StayWarm,
                Economy10 = priceResult.Economy10,
                Cashback = priceResult.Cashback,
                Hide = priceResult.Hide,
                TotalExitFees = priceResult.TotalExitFees,
                ElectricityEstimatedMonthlyCost = priceResult.ElectricityEstimatedMonthlyCost,
                GasEstimatedMonthlyCost = priceResult.GasEstimatedMonthlyCost,
                IsMostPopularTariffForUser = priceResult.IsMostPopularTariffForUser,
                IsCollectiveTariff = priceResult.IsCollectiveTariff,
                IsExclusiveTariff = priceResult.IsExclusiveTariff,
                ShowInRelevancyBox = priceResult.ShowInRelevancyBox,
                ShowStrapline = priceResult.ShowStrapline,
                CheapestBigSupplier = priceResult.CheapestBigSupplier,
                CheapestLongFixed = priceResult.CheapestLongFixed,
                Cheapest = priceResult.Cheapest,
                CheapestMediumFixed = priceResult.CheapestMediumFixed,
                CheapestCanApply = priceResult.CheapestCanApply,
                CheapestGreen = priceResult.CheapestGreen,
                CheapestHighestRated = priceResult.CheapestHighestRated,
                CheapestShortFixed = priceResult.CheapestShortFixed
            };
            return result;
        }

        public static GetPricesRequest ToEhlPriceRequest(this MakeProviderEnquiry<EnergyEnquiry> energyEnquiry)
        {
            var result = new GetPricesRequest()
            {
                JourneyId = energyEnquiry.Enquiry.JourneyId
            };
            return result;
        }
    }
}
