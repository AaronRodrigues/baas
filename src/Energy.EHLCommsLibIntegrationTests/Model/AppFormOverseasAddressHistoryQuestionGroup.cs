using System;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class AppFormOverseasAddressHistoryQuestionGroup : AppFormQuestionGroup
    {
        public AppFormOverseasAddressHistoryQuestionGroup()
        {
        }

        public AppFormOverseasAddressHistoryQuestionGroup(string name, string tags)
            : base(name, tags)
        {

        }

        public int AddressHistoryMonthsRequired { get; set; }
    }
}