const fs = require('fs');
const path = require('path');
const _ = require('lodash');
const args = require('yargs').argv;

const flist = ['mvc-request.json', 'mvc-response.json'];

const files = _.map(flist, fname => JSON.parse(JSON.parse(fs.readFileSync(path.resolve(path.join(args.f, fname))).toString())));

_.each(flist, (json, index) => fs.writeFileSync(path.resolve(path.join(args.f, `${flist[index]}.parsed`)), JSON.stringify(files[index], null, 2)));

const mvcRq = files[0],
    adapterRq = JSON.parse(fs.readFileSync(path.resolve('adapter-request.template.json')));

(function assign(map, prefix){
    const defaultOf = { "string": "", "int": 0, "boolean": false };
    _.each(map, (dest, src) => {
        if(!_.isString(dest) && !_.isArray(dest)) assign(dest, (prefix ? prefix + '.' : '') + src);
        else {
            let valType = 'int';
            if( _.isArray(dest) ){
                valType = dest[1];
                dest = dest[0];
            }
            const destList = dest.split(',');
            _.each(destList, dest => {
                const val = _.get(mvcRq, (prefix ? prefix + '.' : '') + src);
                _.set(adapterRq, 'risk.' + dest, val || defaultOf[valType] );
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
        "MainHeatingSource":["NoBill.MainHeatingSource", "string" ],
        "HeatingUsage":[ "NoBill.HeatingUsage", "string" ],
        "HouseInsulation":[ "NoBill.HouseInsulation", "string" ],
        "MainCookingSource": ["NoBill.MainCookingSource", "string" ],
        "HouseOccupied": ["NoBill.HouseOccupied", "boolean" ],
        "HouseType": ["NoBill.HouseType", "string" ]
    },
    "GasSupplierId": "GasSupplierId",
    "GasTariffId": "GasTariffId",
    "GasPaymentMethodId": "GasPaymentMethodId",
    "ElectricitySupplierId": "ElectricitySupplierId",
    "ElectricityTariffId": "ElectricityTariffId",
    "ElectricityPaymentMethodId": "ElectricityPaymentMethodId",
    "ElectricityEco7": [ "Economy7", "boolean" ],
    "Eco7NightUsageValue": "Economy7NightUsage",
    "IgnoreProRataComparison": [ "IgnoreProRataComparison", "boolean" ],
    "Postcode": "Postcode",
    "SwitchId": "SwitchId",
    "EnergyJourneyType": "EnergyJourneyType"
});

fs.writeFileSync(path.resolve(path.join(args.f, `adapter-request.json`)), JSON.stringify(adapterRq, null, 2));