using System;
using System.Dynamic;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class UnknownAppFormQuestion : AppFormQuestion
    {
        public UnknownAppFormQuestion()
        {
            Type = AppFormQuestionType.Unknown;
        }
    }
}