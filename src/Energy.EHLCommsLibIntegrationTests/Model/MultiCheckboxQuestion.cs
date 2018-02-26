using System;
using System.Collections.Generic;
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

            if (this.AcceptableItems != null && questionItem.Data != null && questionItem.Data.GetType() == typeof (JArray))
            {
                foreach (var selectedItem in (JArray)questionItem.Data)
                {
                    string item = selectedItem.Value<string>();
                    if (selectedValues.Contains(item) == false)
                    {
                        selectedValues.Add(item);
                    }
                }

                foreach (var acceptableItem in this.AcceptableItems)
                {
                    if (selectedValues.Contains(acceptableItem.Id) == true)
                    {
                        acceptableItem.Data = true;
                    }
                }
            }

            Data = null;
        }

        public override string DataBinding
        {
            get
            {
                return string.Format("checked: dynamicData.{0}.{1}.data", GroupName, FieldName);
            }
        }
        
        public override dynamic DynamicData
        {
            get
            {
                var data = base.DynamicData;

                if (data.data == null)
                {
                    data.data = new List<string>();
                }

                return data;
            }
        }
    }
}