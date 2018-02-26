using System;
using System.Linq;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;
using Newtonsoft.Json;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class DropdownQuestion : OptionQuestion
    {
        public DropdownQuestion()
        {
            Type = AppFormQuestionType.DropdownList;
        }

        public override string DataBinding
        {
            get
            {
                if (Dynamic)
                {
                    var optionsBindingText = string.Format("dynamicOptions.{0}.{1}.items", GroupName, FieldName);
                    return string.Format("value: dynamicData.{0}.{1}.data, options: {2}, optionsText: 'name', optionsValue: 'id'", GroupName, FieldName, optionsBindingText);
                }

                return base.DataBinding;
            }
        }

        public override dynamic DynamicData
        {
            get
            {
                var data = base.DynamicData;

                if ((GroupName == "heatCoverBoltOn" && FieldName == "interestedInBoltOn") ||
                    (GroupName == "sseBoltOns" && FieldName == "boltOnProduct"))
                {
                    data.data = "NoThanks";
                }

                return data;
            }
        }

        public bool Dynamic { get; set; }

        public override void Initialise(string groupName, Item questionItem, SignupApiResponse response)
        {
            base.Initialise(groupName, questionItem, response);

            if (groupName == "supplyAddress" && questionItem.Name == "knownAddress")
            {
                //lets map meta data so we can hide display the meter numbers for the selected address
                if (response.SupplyAddressSearchResults != null)
                {
                    AcceptableItems.ForEach(i =>
                    {
                        var result = response.SupplyAddressSearchResults.FirstOrDefault(s => s.Id == i.Id);

                        if (result != null)
                        {
                            i.MetaData = JsonConvert.SerializeObject(new
                            {
                                gasMeterKnown = result.KnownGasMeter,
                                electricityMeterKnown = result.KnownElectricityMeter,
                                igt = result.IGTWarning
                            });
                        }
                    });
                }
            }
            
            Dynamic = (questionItem.Type.Equals("linked"));
        }
    }
}