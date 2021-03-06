﻿using System.Collections.Generic;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models.Prices;
using Energy.EHLCommsLibIntegrationTests.Model;


namespace Energy.EHLCommsLibIntegrationTests.Helpers
{
    public static class EntityHelper
    {
       public static GetPricesRequest GenerateValidPricesRequest(StartSwitchResponse response)
        {
            var request = new GetPricesRequest
            {
                CurrentSupplyUrl = response.CurrentSupplyUrl,
                SwitchUrl = response.SwitchStatusUrl,
                CompareType = CompareWhat.Both,
                GasSupplierId = 59,
                GasTariffId = 301,
                GasPaymentMethodId = 2,
                ElectricitySupplierId = 59,
                ElectricityTariffId = 301,
                ElectricityPaymentMethodId = 2,
                ElectricityEco7 = false,
                PercentageNightUsage = 0,
                UseDetailedEstimatorForElectricity = false,
                UseDetailedEstimatorForGas = false
            };

            request.UsageData.GasKwh = 8000;
            request.UsageData.GasUsagePeriod = UsagePeriod.Quarterly;
            request.UsageData.ElectricityKwh = 8000;
            request.UsageData.ElectricityUsagePeriod = UsagePeriod.Quarterly;

            request.SpendData.GasSpendAmount = 500;
            request.SpendData.GasSpendPeriod = UsagePeriod.Annually;
            request.SpendData.ElectricitySpendAmount = 400;
            request.SpendData.ElectricitySpendPeriod = UsagePeriod.Annually;

            request.EstimatorData.HeatingUsage = "1";
            request.EstimatorData.HouseInsulation = "1";
            request.EstimatorData.HouseOccupied = "1";
            request.EstimatorData.HouseType = "4";
            request.EstimatorData.MainCookingSource = "1";
            request.EstimatorData.MainHeatingSource = "0";
            request.EstimatorData.NumberOfBedrooms = "4";
            request.EstimatorData.NumberOfOccupants = "4";

            return request;
        }

    }
}
