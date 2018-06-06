const fs = require('fs');
const path = require('path');
const _ = require('lodash');
const args = require('yargs').argv;

const flist = ['mvc-request.json', 'mvc-response.json'];

const files = _.map(flist, fname => JSON.parse(JSON.parse(fs.readFileSync(path.resolve(path.join(args.f, fname))).toString())));

_.each(flist, (json, index) => fs.writeFileSync(path.resolve(path.join(args.f, `${flist[index]}.parsed`)), JSON.stringify(files[index], null, 2)));

const mvcRq = files[0],
    adapterRq = JSON.parse(fs.readFileSync(path.resolve(path.join(args.f, 'adapter-request.template.json'))));

(function assign(map, prefix){
    _.each(map, (dest, src) => {
        if(!_.isString(dest)) assign(dest, (prefix ? prefix + '.' : '') + src);
        else {
            const destList = dest.split(',');
            _.each(destList, dest => {
                _.set(adapterRq, 'risk.' + dest, _.get(mvcRq, (prefix ? prefix + '.' : '') + src));
            });
        }
    });
})({
    "CurrentSupplyUrl": "CurrentSupplyUrl",
    "SwitchUrl": "SwitchUrl",
    "CompareType": "CompareType",
    "UsageData": {
        "GasKwh": "Bill.GasUsage",
        "GasUsagePeriod": "Bill.GasUsagePeriod",
        "ElectricityKwh": "Bill.ElectricityUsage",
        "ElectricityUsagePeriod": "Bill.ElectricityUsagePeriod"
    },
    "SpendData": {
        "GasSpendAmount": "Bill.GasSpend,NoBill.GasSpend",
        "GasSpendPeriod": "Bill.GasSpendPeriod,NoBill.GasSpendPeriod",
        "ElectricitySpendAmount": "Bill.ElectricitySpend,NoBill.ElectricitySpend",
        "ElectricitySpendPeriod": "Bill.ElectricitySpendPeriod,NoBill.ElectricitySpendPeriod"
    },
    "EstimatorData": {
        "NumberOfBedrooms": "NoBill.NumberOfBedrooms",
        "NumberOfOccupants": "NoBill.NumberOfOccupants",
        "MainHeatingSource": "NoBill.MainHeatingSource",
        "HeatingUsage": "NoBill.HeatingUsage",
        "HouseInsulation": "NoBill.HouseInsulation",
        "MainCookingSource": "NoBill.MainCookingSource",
        "HouseOccupied": "NoBill.HouseOccupied",
        "HouseType": "NoBill.HouseType"
    },
    "GasSupplierId": "GasSupplierId",
    "GasTariffId": "GasTariffId",
    "GasPaymentMethodId": "GasPaymentMethodId",
    "ElectricitySupplierId": "ElectricitySupplierId",
    "ElectricityTariffId": "ElectricityTariffId",
    "ElectricityPaymentMethodId": "ElectricityPaymentMethodId",
    "ElectricityEco7": "Economy7",
    "Eco7NightUsageValue": "Economy7NightUsage",
    "IgnoreProRataComparison": "IgnoreProRataComparison",
    "Postcode": "Postcode",
    "SwitchId": "SwitchId",
    "EnergyJourneyType": "EnergyJourneyType"
});

fs.writeFileSync(path.resolve(path.join(args.f, `adapter-request.json`)), JSON.stringify(adapterRq, null, 2));