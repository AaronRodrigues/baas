using System;
using System.Dynamic;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class CheckboxQuestion : AppFormQuestion
    {
        public CheckboxQuestion()
        {
            Type = AppFormQuestionType.Bool;
        }

        public override string DataBinding => string.Format("checked: dynamicData.{0}.{1}.data", GroupName, FieldName);

        public override dynamic DynamicData
        {
            get
            {
                dynamic field = new ExpandoObject();

                field.fieldName = FieldName;

                if (string.IsNullOrWhiteSpace(Data?.ToString()))
                    field.data = false;
                else
                    field.data = Data;

                return field;
            }
        }
    }
}