using System;
using System.Dynamic;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class ResidencyPeriodQuestion : AppFormQuestion
    {
        public ResidencyPeriodQuestion()
        {
            Type = AppFormQuestionType.ResidencyPeriod;
        }

        public override dynamic DynamicData
        {
            get
            {
                dynamic field = new ExpandoObject();
                field.data = Data;

                field.years = new ExpandoObject();
                field.years.data = GetYears();

                field.months = new ExpandoObject();
                field.months.data = GetMonths();

                return field;
            }
        }

        private string GetYears()
        {
            if (Data is long)
            {
                var totalMonths = (long) Data;
                
                //convert the number of months passed back into years and months
                var years = totalMonths/12;

                if (years > 0)
                {
                    return years.ToString();
                }
            }

            return "";
        }
        
        private string GetMonths()
        {
            if (Data is long)
            {
                var totalMonths = (long)Data;
                
                //convert the number of months passed back into years and months
                var months = totalMonths % 12;

                if (months > 0)
                {
                    return months.ToString();
                }
            }

            return "";
        }
    }
}