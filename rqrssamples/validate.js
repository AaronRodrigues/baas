const fs = require('fs');
const path = require('path');
const _ = require('lodash');
const args = require('yargs').argv;


const mvcResp = JSON.parse(fs.readFileSync(path.resolve(path.join(args.f, 'mvc-response.json.parsed'))));
const adapterResp = JSON.parse(fs.readFileSync(path.resolve(path.join(args.f, 'adapter-response.json'))));

const mvcPriceList = _.map(mvcResp.Prices, 'ResultId')
const adaperPriceList = _.map(adapterResp.quotes, 'resultId');

console.log(mvcPriceList.length, adaperPriceList.length);
console.log(_.xor(mvcPriceList, adaperPriceList))

