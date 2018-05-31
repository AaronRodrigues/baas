using System;
using System.Collections.Generic;
using System.Linq;
using CTM.Quoting.Provider;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models.Prices;
using Energy.ProviderAdapter.Models;

//To DO: Move models and enums to separate library
//To DO: Change BrandCode logic

namespace Energy.ProviderAdapter.ModelConverters
{
    public static class EhlModelsConverterHelper
    {
        public static EnergyQuote ToEnergyQuote(this PriceResult priceResult)
        {
            return new EnergyQuote
            {
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
            var risk = enquiry.Risk;

            GetPricesRequest request = new GetPricesRequest()
            {
                EnergyJourneyType = risk.EnergyJourneyType,
                JourneyId = Guid.Parse(risk.JourneyId),
                Postcode = risk.Postcode,
                SwitchId = risk.SwitchId,

                CurrentSupplyUrl = risk.CurrentSupplyUrl,
                SwitchUrl = risk.SwitchUrl,
                PrePayment = risk.PrePayment,
                CompareType = risk.CompareType,

                GasSupplierId = risk.GasSupplierId,
                GasTariffId = risk.GasTariffId,
                GasPaymentMethodId = risk.GasPaymentMethodId,
                ElectricitySupplierId = risk.ElectricitySupplierId,
                ElectricityTariffId = risk.ElectricityTariffId,
                ElectricityPaymentMethodId = risk.ElectricityPaymentMethodId,
                ElectricityEco7 = risk.Economy7,
                PercentageNightUsage = risk.Economy7NightUsage,
                IgnoreProRataComparison = risk.IgnoreProRataComparison
            };

            if (risk.CompareType == CompareWhat.Gas) request.UseDetailedEstimatorForElectricity = false;
            else request.UseDetailedEstimatorForElectricity = risk.NoBill != null && risk.NoBill.ElectricitySpendUnknown;

            if (risk.CompareType == CompareWhat.Electricity) request.UseDetailedEstimatorForGas = false;
            else request.UseDetailedEstimatorForGas = risk.NoBill != null && risk.NoBill.GasSpendUnknown;


            if (risk.EnergyJourneyType == EnergyJourneyType.HaveMyBill && risk.Bill != null)
            {

                request.UsageData.GasKwh = risk.Bill.GasUsage;
                request.UsageData.GasUsagePeriod = risk.Bill.GasUsagePeriod;
                request.UsageData.ElectricityKwh = risk.Bill.ElectricityUsage;
                request.UsageData.ElectricityUsagePeriod = risk.Bill.ElectricityUsagePeriod;

                request.SpendData.GasSpendAmount = risk.Bill.GasSpend;
                request.SpendData.GasSpendPeriod = risk.Bill.GasSpendPeriod;
                request.SpendData.ElectricitySpendAmount = risk.Bill.ElectricitySpend;
                request.SpendData.ElectricitySpendPeriod = risk.Bill.ElectricitySpendPeriod;

            }

            if (risk.EnergyJourneyType == EnergyJourneyType.DontHaveMyBill && risk.NoBill != null)
            {

                request.SpendData.GasSpendAmount = risk.NoBill.GasSpend;
                request.SpendData.GasSpendPeriod = risk.NoBill.GasSpendPeriod;
                request.SpendData.ElectricitySpendAmount = risk.NoBill.ElectricitySpend;
                request.SpendData.ElectricitySpendPeriod = risk.NoBill.ElectricitySpendPeriod;

                request.EstimatorData.NumberOfBedrooms = risk.NoBill.NumberOfBedrooms.ToString();
                request.EstimatorData.NumberOfOccupants = risk.NoBill.NumberOfOccupants.ToString();
                request.EstimatorData.MainHeatingSource = risk.NoBill.MainHeatingSource;
                request.EstimatorData.HeatingUsage = risk.NoBill.HeatingUsage;
                request.EstimatorData.HouseInsulation = risk.NoBill.HouseInsulation;
                request.EstimatorData.MainCookingSource = risk.NoBill.MainCookingSource;
                request.EstimatorData.HouseOccupied = risk.NoBill.HouseOccupied.ToString();
                request.EstimatorData.HouseType = risk.NoBill.HouseType;
            }

            return request;
        }

        public static List<EnergyQuote> AddFakeBrandCode(this List<EnergyQuote> quoteList, string brandCode)
        {
            for (var i = 0; i < quoteList.Count; i++)
            {
                quoteList[i].Brand = brandCode + i;
            }
            return quoteList;
        }

    }
}
