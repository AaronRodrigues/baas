using System.Linq;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Extensions
{
    public static class GetPricesRequestExtensions
    {
        public static GetPricesRequest PopulateCurrentSupplyWithRequestData(this GetPricesRequest request,
            ApiResponse currentSupplyTemplate)
        {
            var compareGas = request.CompareType != CompareWhat.Electricity;
            var compareElec = request.CompareType != CompareWhat.Gas;

            UpdateItemData(currentSupplyTemplate, "includedFuels", "compareGas",
                compareGas.ToString());
            UpdateItemData(currentSupplyTemplate, "includedFuels", "compareElec",
                compareElec.ToString());

            UpdateItemData(currentSupplyTemplate, "gasTariff", "supplier",
                request.GasSupplierId.ToString());
            UpdateItemData(currentSupplyTemplate, "gasTariff", "supplierTariff",
                request.GasTariffId.ToString());
            UpdateItemData(currentSupplyTemplate, "gasTariff", "paymentMethod",
                request.GasPaymentMethodId.ToString());

            UpdateItemData(currentSupplyTemplate, "elecTariff", "supplier",
                request.ElectricitySupplierId.ToString());
            UpdateItemData(currentSupplyTemplate, "elecTariff", "supplierTariff",
                request.ElectricityTariffId.ToString());
            UpdateItemData(currentSupplyTemplate, "elecTariff", "paymentMethod",
                request.ElectricityPaymentMethodId.ToString());
            UpdateItemData(currentSupplyTemplate, "elecTariff", "economy7",
                request.ElectricityEco7.ToString());

            return request;
        }

        public static GetPricesRequest PopulatePreferencesWithRequestData(this GetPricesRequest request,
            ApiResponse preferencesTemplate)
        {
            const string filterOptionAll = "105";
            const string paymentMethodAny = "-1";
            UpdateItemData(preferencesTemplate, "tariffFilterOptions", "tariffFilterOption",
                filterOptionAll);
            if (request.PrePayment == "false")
                UpdateItemData(preferencesTemplate, "limitToPaymentType", "paymentMethod",
                    paymentMethodAny);

            return request;
        }

        public static GetPricesRequest PopulateUsageWithRequestData(this GetPricesRequest request,
            ApiResponse usageTemplate)
        {
            var compareGas = request.CompareType != CompareWhat.Electricity;
            var compareElec = request.CompareType != CompareWhat.Gas;

            UpdateItemData(usageTemplate, "includedFuels", "compareGas", compareGas.ToString());
            UpdateItemData(usageTemplate, "includedFuels", "compareElec", compareElec.ToString());

            switch (request.EnergyJourneyType)
            {
                case EnergyJourneyType.HaveMyBill:
                case EnergyJourneyType.Qr:
                    if (request.CalculateElecBasedOnBillSpend)
                    {
                        PopulateSpendUsageForElectricity(request, usageTemplate);
                    }
                    else
                    {
                        PopulateKWhUsage(request, usageTemplate);
                    }

                    if (request.CalculateGasBasedOnBillSpend)
                    {
                        PopulateSpendUsageForGas(request, usageTemplate);
                    }
                    else
                    {
                        PopulateKWhUsage(request, usageTemplate);
                    }

                    break;

                case EnergyJourneyType.DontHaveMyBill:
                    if (request.UseDetailedEstimatorForElectricity)
                        PopulateDetailedUsageForElectricity(request, usageTemplate);
                    else
                        PopulateSpendUsageForElectricity(request, usageTemplate);

                    if (request.UseDetailedEstimatorForGas)
                        PopulateDetailedUsageForGas(request, usageTemplate);
                    else
                        PopulateSpendUsageForGas(request, usageTemplate);

                    break;
            }

            if (compareElec && request.ElectricityEco7)
            {
                UpdateItemData(usageTemplate, "economy7", "nightUsagePercentage",
                    request.PercentageNightUsage.ToString());
            }
            return request;
        }

        private static void PopulateSpendUsageForElectricity(GetPricesRequest request, ApiResponse usageTemplate)
        {
            var compareElec = request.CompareType != CompareWhat.Gas;
            if (!compareElec) return;
            UpdateItemData(usageTemplate, "elecUsageType", "usageType", EhlApiConstants.UsageTypeBySpend);
            UpdateItemData(usageTemplate, "elecSpend", "usageAsSpend",
                request.SpendData.ElectricitySpendAmount.ToString());
            UpdateItemData(usageTemplate, "elecSpend", "spendPeriod",
                ((int) request.SpendData.ElectricitySpendPeriod).ToString());
        }

        private static void PopulateKWhUsage(GetPricesRequest request, ApiResponse usageTemplate)
        {
            if (!request.CalculateGasBasedOnBillSpend)
            {
                UpdateItemData(usageTemplate, "gasUsageType", "usageType", EhlApiConstants.UsageTypeByKWhUsage);
                UpdateItemData(usageTemplate, "gasKWhUsage", "usageAsKWh", request.UsageData.GasKwh.ToString());
                UpdateItemData(usageTemplate, "gasKWhUsage", "usagePeriod",
                    ((int) request.UsageData.GasUsagePeriod).ToString());
            }

            if (request.CalculateElecBasedOnBillSpend) return;
            UpdateItemData(usageTemplate, "elecUsageType", "usageType", EhlApiConstants.UsageTypeByKWhUsage);
            UpdateItemData(usageTemplate, "elecKWhUsage", "usageAsKWh", request.UsageData.ElectricityKwh.ToString());
            UpdateItemData(usageTemplate, "elecKWhUsage", "usagePeriod",
                ((int) request.UsageData.ElectricityUsagePeriod).ToString());
        }

        private static void PopulateSpendUsageForGas(GetPricesRequest request, ApiResponse usageTemplate)
        {
            var compareGas = request.CompareType != CompareWhat.Electricity;
            if (compareGas)
            {
                UpdateItemData(usageTemplate, "gasUsageType", "usageType", EhlApiConstants.UsageTypeBySpend);
                UpdateItemData(usageTemplate, "gasSpend", "usageAsSpend", request.SpendData.GasSpendAmount.ToString());
                UpdateItemData(usageTemplate, "gasSpend", "spendPeriod",
                    ((int) request.SpendData.GasSpendPeriod).ToString());
            }
        }

        private static void PopulateDetailedUsageForElectricity(GetPricesRequest request,
            ApiResponse usageTemplate)
        {
            UpdateItemData(usageTemplate, "elecUsageType", "usageType", EhlApiConstants.UsageTypeByEstimator);
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "houseType", request.EstimatorData.HouseType);
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "numberOfBedrooms",
                request.EstimatorData.NumberOfBedrooms);
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "mainCookingSource",
                request.EstimatorData.MainCookingSource);
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "cookingFrequency",
                ((int) CookingFrequency.Daily).ToString());
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "centralHeating",
                request.EstimatorData.MainHeatingSource);
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "numberOfOccupants",
                request.EstimatorData.NumberOfOccupants);
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "insulation", request.EstimatorData.HouseInsulation);
            UpdateItemData(usageTemplate, "elecDetailedEstimate", "energyUsage", request.EstimatorData.HouseOccupied);
        }

        private static void PopulateDetailedUsageForGas(GetPricesRequest request, ApiResponse usageTemplate)
        {
            UpdateItemData(usageTemplate, "gasUsageType", "usageType", EhlApiConstants.UsageTypeByEstimator);
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "houseType", request.EstimatorData.HouseType);
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "numberOfBedrooms",
                request.EstimatorData.NumberOfBedrooms);
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "mainCookingSource",
                request.EstimatorData.MainCookingSource);
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "cookingFrequency",
                ((int) CookingFrequency.Daily).ToString());
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "centralHeating",
                request.EstimatorData.MainHeatingSource);
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "numberOfOccupants",
                request.EstimatorData.NumberOfOccupants);
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "insulation", request.EstimatorData.HouseInsulation);
            UpdateItemData(usageTemplate, "gasDetailedEstimate", "energyUsage", request.EstimatorData.HouseOccupied);
        }

        private static void UpdateItemData(ApiResponse currentSupplyTemplate, string groupName, string itemName,
            string value)
        {
            currentSupplyTemplate.DataTemplate.Groups
                .First(g => g.Name.Equals(groupName))
                .Items.First(i => i.Name.Equals(itemName))
                .Data = value;
        }

        private static void UpdateItemData(ApiResponse currentSupplyTemplate, string groupName, string itemName,
            int value)
        {
            currentSupplyTemplate.DataTemplate.Groups
                .First(g => g.Name.Equals(groupName))
                .Items.First(i => i.Name.Equals(itemName))
                .Data = value;
        }

        private static void UpdateItemData(ApiResponse currentSupplyTemplate, string groupName, string itemName,
            bool value)
        {
            currentSupplyTemplate.DataTemplate.Groups
                .First(g => g.Name.Equals(groupName))
                .Items.First(i => i.Name.Equals(itemName))
                .Data = value;
        }

    }
}