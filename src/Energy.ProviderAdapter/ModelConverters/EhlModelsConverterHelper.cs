using System.Linq;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Models.Prices;
using Energy.ProviderAdapter.Models;

//To DO: Move models and enums to separate library


namespace Energy.ProviderAdapter.ModelConverters
{
    public static class EhlModelsConverterHelper
    {
        public static EnergyQuote ToEnergyQuote(this PriceResult priceResult, string brandCode)
        {
            return new EnergyQuote
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
        }

        public static GetPricesRequest ToEhlPriceRequest(this MakeProviderEnquiry<EnergyEnquiry> energyEnquiry)
        {
            var getPriceRequest = new GetPricesRequest()
            {
                JourneyId = energyEnquiry.Enquiry.JourneyId,
                CurrentSupplyUrl = energyEnquiry.Enquiry.CurrentSupplyUrl,
                UseDetailedEstimatorForElectricity = energyEnquiry.Enquiry.UseDetailedEstimatorForElectricity,
                UseDetailedEstimatorForGas = energyEnquiry.Enquiry.UseDetailedEstimatorForGas,
                IgnoreProRataComparison = energyEnquiry.Enquiry.IgnoreProRataComparison,
                SwitchUrl = energyEnquiry.Enquiry.SwitchUrl,
                PrePayment = energyEnquiry.Enquiry.PrePayment,
                CompareType = energyEnquiry.Enquiry.CompareType,
                GasSupplierId = energyEnquiry.Enquiry.GasSupplierId,
                GasTariffId = energyEnquiry.Enquiry.GasTariffId,
                GasPaymentMethodId = energyEnquiry.Enquiry.GasPaymentMethodId,
                ElectricitySupplierId = energyEnquiry.Enquiry.ElectricitySupplierId,
                ElectricityTariffId = energyEnquiry.Enquiry.ElectricityTariffId,
                ElectricityPaymentMethodId = energyEnquiry.Enquiry.ElectricityPaymentMethodId,
                ElectricityEco7 = energyEnquiry.Enquiry.ElectricityEco7,
                PercentageNightUsage = energyEnquiry.Enquiry.PercentageNightUsage,
                TariffCustomFeatureEnabled = energyEnquiry.Enquiry.TariffCustomFeatureEnabled,
                CustomFeatures = energyEnquiry.Enquiry.CustomFeatures

            };
            getPriceRequest.UsageData.FromEnquiry(energyEnquiry.Enquiry.UsageData);
            getPriceRequest.SpendData.FromEnquiry(energyEnquiry.Enquiry.SpendData);
            getPriceRequest.EstimatorData.FromEnquiry(energyEnquiry.Enquiry.EstimatorData);
            return getPriceRequest;
        }

        private static void FromEnquiry(this EHLCommsLib.Models.UsageData usageData, UsageData enquiryUsageData)
        {
            usageData.ElectricityKwh = enquiryUsageData.ElectricityKwh;
            usageData.GasKwh = enquiryUsageData.GasKwh;
            usageData.GasUsagePeriod = enquiryUsageData.GasUsagePeriod;
            usageData.ElectricityUsagePeriod = enquiryUsageData.ElectricityUsagePeriod;
        }

        private static void FromEnquiry(this EHLCommsLib.Models.SpendData spendData, SpendData enquerySpendData)
        {
            spendData.GasSpendAmount = enquerySpendData.GasSpendAmount;
            spendData.GasSpendPeriod = enquerySpendData.GasSpendPeriod;
            spendData.ElectricitySpendAmount = enquerySpendData.ElectricitySpendAmount;
            spendData.ElectricitySpendPeriod = enquerySpendData.ElectricitySpendPeriod;
        }

        private static void FromEnquiry(this EHLCommsLib.Models.EstimatorData spendData, EstimatorData enquerySpendData)
        {
            spendData.NumberOfBedrooms = enquerySpendData.NumberOfBedrooms;
            spendData.NumberOfOccupants = enquerySpendData.NumberOfOccupants;
            spendData.MainHeatingSource = enquerySpendData.MainHeatingSource;
            spendData.HeatingUsage = enquerySpendData.HeatingUsage;
            spendData.HouseInsulation = enquerySpendData.HouseInsulation;
            spendData.MainCookingSource = enquerySpendData.MainCookingSource;
            spendData.HouseOccupied = enquerySpendData.HouseOccupied;
            spendData.HouseType = enquerySpendData.HouseType;
        }
    }
}
