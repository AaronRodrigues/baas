using System;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class TextboxQuestion : AppFormQuestion
    {
        public TextboxQuestion()
        {
            Type = AppFormQuestionType.Text;
        }

        public TextboxQuestion(bool password)
        {
            Type = password?AppFormQuestionType.Password : AppFormQuestionType.Text;
        }

        public override dynamic DynamicData
        {
            get
            {
                var data =  base.DynamicData;

                if (Type == AppFormQuestionType.Password)
                {
                    data.data = "";
                }

                return data;
            }
        }
    }
}