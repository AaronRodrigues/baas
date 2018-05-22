using System.Collections.Generic;
using System.Linq;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models;
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

            var enquiry = energyEnquiry.Enquiry;

            GetPricesRequest request = new GetPricesRequest()
            {
                EnergyJourneyType = enquiry.EnergyJourneyType,
                JourneyId = enquiry.JourneyId,
                Postcode = enquiry.Postcode,
                SwitchId = enquiry.SwitchId,

                CurrentSupplyUrl = enquiry.CurrentSupplyUrl,
                SwitchUrl = enquiry.SwitchUrl,
                PrePayment = enquiry.PrePayment,
                CompareType = enquiry.CompareType,

                GasSupplierId = enquiry.GasSupplierId,
                GasTariffId = enquiry.GasTariffId,
                GasPaymentMethodId = enquiry.GasPaymentMethodId,
                ElectricitySupplierId = enquiry.ElectricitySupplierId,
                ElectricityTariffId = enquiry.ElectricityTariffId,
                ElectricityPaymentMethodId = enquiry.ElectricityPaymentMethodId,
                ElectricityEco7 = enquiry.Economy7,
                PercentageNightUsage = enquiry.Economy7NightUsage,
                IgnoreProRataComparison = enquiry.IgnoreProRataComparison
            };

            if (enquiry.CompareType == CompareWhat.Gas) request.UseDetailedEstimatorForElectricity = false;
            else request.UseDetailedEstimatorForElectricity = enquiry.NoBill != null && enquiry.NoBill.ElectricitySpendUnknown;

            if (enquiry.CompareType == CompareWhat.Electricity) request.UseDetailedEstimatorForGas = false;
            else request.UseDetailedEstimatorForGas = enquiry.NoBill != null && enquiry.NoBill.GasSpendUnknown;


            if (enquiry.EnergyJourneyType == EnergyJourneyType.HaveMyBill && enquiry.Bill != null)
            {

                request.UsageData.GasKwh = enquiry.Bill.GasUsage;
                request.UsageData.GasUsagePeriod = enquiry.Bill.GasUsagePeriod;
                request.UsageData.ElectricityKwh = enquiry.Bill.ElectricityUsage;
                request.UsageData.ElectricityUsagePeriod = enquiry.Bill.ElectricityUsagePeriod;

                request.SpendData.GasSpendAmount = enquiry.Bill.GasSpend;
                request.SpendData.GasSpendPeriod = enquiry.Bill.GasSpendPeriod;
                request.SpendData.ElectricitySpendAmount = enquiry.Bill.ElectricitySpend;
                request.SpendData.ElectricitySpendPeriod = enquiry.Bill.ElectricitySpendPeriod;

            }

            if (enquiry.EnergyJourneyType == EnergyJourneyType.DontHaveMyBill && enquiry.NoBill != null)
            {

                request.SpendData.GasSpendAmount = enquiry.NoBill.GasSpend;
                request.SpendData.GasSpendPeriod = enquiry.NoBill.GasSpendPeriod;
                request.SpendData.ElectricitySpendAmount = enquiry.NoBill.ElectricitySpend;
                request.SpendData.ElectricitySpendPeriod = enquiry.NoBill.ElectricitySpendPeriod;

                request.EstimatorData.NumberOfBedrooms = enquiry.NoBill.NumberOfBedrooms.ToString();
                request.EstimatorData.NumberOfOccupants = enquiry.NoBill.NumberOfOccupants.ToString();
                request.EstimatorData.MainHeatingSource = enquiry.NoBill.MainHeatingSource;
                request.EstimatorData.HeatingUsage = enquiry.NoBill.HeatingUsage;
                request.EstimatorData.HouseInsulation = enquiry.NoBill.HouseInsulation;
                request.EstimatorData.MainCookingSource = enquiry.NoBill.MainCookingSource;
                request.EstimatorData.HouseOccupied = enquiry.NoBill.HouseOccupied.ToString();
                request.EstimatorData.HouseType = enquiry.NoBill.HouseType;
            }

            return request;
        }

    }
}
