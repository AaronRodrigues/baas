using System;
using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;
using Newtonsoft.Json.Linq;


namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class MultiCheckboxQuestion : OptionQuestion
    {
        public MultiCheckboxQuestion()
        {
            Type = AppFormQuestionType.MultiBool;
        }

        public override void Initialise(string groupName, Item questionItem, SignupApiResponse response)
        {
            base.Initialise(groupName, questionItem, response);

            HashSet<string> selectedValues = new HashSet<string>();

            if (AcceptableItems != null && questionItem.Data != null && questionItem.Data.GetType() == typeof (JArray))
            {
                foreach (string item in ((JArray)questionItem.Data).Select(selectedItem => selectedItem.Value<string>()).Where(item => selectedValues.Contains(item) == false))
                {
                    selectedValues.Add(item);
                }

                foreach (var acceptableItem in AcceptableItems.Where(acceptableItem => selectedValues.Contains(acceptableItem.Id)))
                {
                    acceptableItem.Data = true;
                }
            }

            Data = null;
        }

        public override string DataBinding => string.Format("checked: dynamicData.{0}.{1}.data", GroupName, FieldName);

        public override dynamic DynamicData
        {
            get
            {
                var data = base.DynamicData ?? new List<string>();

                return data;
            }
        }
    }
}