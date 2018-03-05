using System;
using System.Dynamic;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{

    [Serializable]
    public class AddressLookupQuestion : AppFormQuestion
    {
        public AddressLookupQuestion()
        {
            Type = AppFormQuestionType.AddressLookup;
        }

        public string LinkedUrl { get; set; }
        public string ButtonText { get; set; }
        public string TargetFieldName
        {
            get
            {
                if (GroupName.ToLower() == "correspondenceaddress")
                {
                    return "knownAddress";
                }

                return FieldName.ToLower() == "secondpreviouspostcode" ? "knownSecondPreviousAddress" : "knownfirstPreviousAddress";
            }
        }

        public override dynamic DynamicData
        {
            get
            {
                dynamic field = new ExpandoObject();

                field.data = Data;
                field.linkedUrl = LinkedUrl;

                return field;
            }
        }

        public override void Initialise(string groupName, Item questionItem, SignupApiResponse response)
        {
            base.Initialise(groupName, questionItem, response);
            LinkedUrl = GetLinkedUrl(questionItem.ValidateAs, response);
        }
    }
}