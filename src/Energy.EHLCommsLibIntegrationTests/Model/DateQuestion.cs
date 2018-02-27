using System;
using System.Dynamic;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class DateQuestion : AppFormQuestion
    {
        public DateQuestion()
        {
            Type = AppFormQuestionType.Date;
        }

        public override dynamic DynamicData
        {
            get
            {
                DateTime? dob = null;
                dynamic field = new ExpandoObject();

                if (!string.IsNullOrEmpty(Data.ToString()))
                {
                    dob = DateTime.Parse(Data.ToString());
                }

                field.data = dob?.ToString("yyyy-MM-dd") ?? string.Empty;

                field.day = new ExpandoObject();
                field.day.data = dob?.Day ?? 1;

                field.month = new ExpandoObject();
                field.month.data = dob?.Month ?? 1;

                field.year = new ExpandoObject();
                field.year.data = dob?.Year ?? 0;

                return field;
            }
        }
    }
}