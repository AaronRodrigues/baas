using System.Collections.Generic;
using System.Linq;
using CTM.Common.BuildingBlocks.Extensions;
using Energy.EHLCommsLib.Contracts.FutureSupplies;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Extensions;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Mappers
{
    public static class PriceResultsMapper
    {
        public static List<PriceResult> MapToPriceResults(this FutureSupplies ehlResults, GetPricesRequest request,
            Dictionary<string, string> customFeatures, bool tariffCustomFeatureEnabled)
        {
            var mappedResults = new List<PriceResult>();

            var ehlResultsSet =
                ehlResults.Results.First(r => r.SupplyType.Id.Equals(request.CompareType.EhlSupplyType()));

            foreach (var supply in ehlResultsSet.EnergySupplies)
            {
                var paymentMethod = int.Parse(supply.SupplyDetails.PaymentMethod.Id);
                var priceResult = new PriceResult
                {
                    AnnualSavings = decimal.Round(supply.ExpectedAnnualSavings, 0),
                    AnnualSpend = decimal.Round(supply.ExpectedAnnualSpend, 0),
                    ElectricityAnnualSavings = supply.ExpectedElecAnnualSavings,
                    ElectricityAnnualSpend = supply.ExpectedElecAnnualSpend,
                    ElectricityEstimatedMonthlyCost = supply.EstimatedElecMonthlyCost,
                    GasAnnualSavings = supply.ExpectedGasAnnualSavings,
                    GasAnnualSpend = supply.ExpectedGasAnnualSpend,
                    GasEstimatedMonthlyCost = supply.EstimatedGasMonthlyCost,
                    ResultId = supply.Id,
                    SupplierId = int.Parse(supply.Supplier.Id),
                    SupplierName = supply.Supplier.Name,
                    SupplierRating = supply.Supplier.ServiceStarRating,
                    TariffId = int.Parse(supply.SupplyDetails.Id),
                    TariffName = supply.SupplyDetails.Name,
                    TariffDetailsUrl = supply.SupplyDetails.FurtherDetails.Uri,
                    PaymentMethod = (PaymentMethodType) paymentMethod,
                    PaymentMethodId = paymentMethod,
                    CanApply = supply.CanApply,
                    CappedOrFixed = supply.SupplyDetails.Attributes.Any(a => a.Equals("CappedOrFixed")),
                    Green = supply.SupplyDetails.Attributes.Any(a => a.Equals("Green")),
                    AccurateMonthlyBilling =
                        supply.SupplyDetails.Attributes.Any(a => a.Equals("AccurateMonthlyBilling")),
                    StayWarm = supply.SupplyDetails.Attributes.Any(a => a.Equals("StayWarm")),
                    Economy10 = supply.SupplyDetails.Attributes.Any(a => a.Equals("Economy10")),
                    PaperLessBilling = supply.SupplyDetails.Attributes.Any(a => a.Equals("PaperlessBilling")),
                    PaperBilling = supply.SupplyDetails.Attributes.Any(a => a.Equals("PaperBilling")),
                    NoStandingCharges = supply.SupplyDetails.Attributes.Any(a => a.Equals("NoStandingCharges")),
                    RenewableFuelPercentage = supply.SupplyDetails.RenewableFuelPercentage,
                    TotalExitFees =
                        CalculateTotalExitFees(request.CompareType, supply.SupplyDetails.ExitFeesGas,
                            supply.SupplyDetails.ExitFeesElectricity),
                    CheapestBigSupplier = PromotionsValidator(supply.Promotions, "CheapestBigSupplier"),
                    CheapestLongFixed = PromotionsValidator(supply.Promotions, "CheapestLongFixed"),
                    CheapestShortFixed = PromotionsValidator(supply.Promotions, "CheapestShortFixed"),
                    Cheapest = PromotionsValidator(supply.Promotions, "Cheapest"),
                    CheapestMediumFixed = PromotionsValidator(supply.Promotions, "CheapestMediumFixed"),
                    CheapestCanApply = PromotionsValidator(supply.Promotions, "CheapestCanApply"),
                    CheapestGreen = PromotionsValidator(supply.Promotions, "CheapestGreen"),
                    CheapestHighestRated = PromotionsValidator(supply.Promotions, "CheapestHighestRated")
                };

                priceResult.SupplierRating = priceResult.SupplierRating > 5 ? 5 : priceResult.SupplierRating;

                // Key features
                priceResult.KeyFeatures = supply.SupplyDetails.KeyFeatures.ToPriceFeatures();

                // Set CTM custom feature text
                if (tariffCustomFeatureEnabled)
                {
                    priceResult.CustomFeatureText =
                        SetCustomFeatureText(priceResult.SupplierName + priceResult.TariffName, customFeatures);
                    priceResult.HasTariffCustomFeature = priceResult.CustomFeatureText != null;
                }


                mappedResults.Add(priceResult);
            }

            return mappedResults;
        }

        private static decimal CalculateTotalExitFees(CompareWhat compareType, decimal gasFee, decimal elecFee)
        {
            switch (compareType)
            {
                case CompareWhat.Both:
                    return gasFee + elecFee;
                case CompareWhat.Electricity:
                    return elecFee;
                case CompareWhat.Gas:
                    return gasFee;
                default:
                    return 0m;
            }
        }

        private static bool PromotionsValidator(List<string> promotions, string name)
        {
            return promotions.IsNotNull() && promotions.IsNotEmpty() && promotions.Contains(name);
        }

        private static string SetCustomFeatureText(string tariffKey, Dictionary<string, string> customFeatures)
        {
            string customFeatureText = null;

            if (customFeatures != null)
                customFeatures.TryGetValue(tariffKey.Replace(" ", "").ToLower(), out customFeatureText);

            return customFeatureText;
        }
    }
}