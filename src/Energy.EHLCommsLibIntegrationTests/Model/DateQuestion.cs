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

                field.data = dob.HasValue ? dob.Value.ToString("yyyy-MM-dd") : string.Empty;

                field.day = new ExpandoObject();
                field.day.data = dob.HasValue ? dob.Value.Day : 1;

                field.month = new ExpandoObject();
                field.month.data = dob.HasValue ? dob.Value.Month : 1;

                field.year = new ExpandoObject();
                field.year.data = dob.HasValue ? dob.Value.Year : 0;

                return field;
            }
        }
    }
}