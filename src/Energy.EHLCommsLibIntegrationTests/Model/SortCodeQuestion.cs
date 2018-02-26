using System;
using System.Dynamic;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class SortCodeQuestion : AppFormQuestion
    {
        public SortCodeQuestion()
        {
            Type = AppFormQuestionType.SortCode;
        }

        public string ValidationUrl { get; set; }

        public override dynamic DynamicData
        {
            get
            {
                var field = base.DynamicData;

                field.validationUrl = ValidationUrl;

                field.partOne = new ExpandoObject();
                field.partOne.data = "";

                field.partTwo = new ExpandoObject();
                field.partTwo.data = "";

                field.partThree = new ExpandoObject();
                field.partThree.data = "";

                return field;
            }
        }


        public override void Initialise(string groupName, Item questionItem, SignupApiResponse response)
        {
            base.Initialise(groupName, questionItem, response);
            ValidationUrl = GetLinkedUrl(questionItem.ValidateAs, response);
        }

    }
}