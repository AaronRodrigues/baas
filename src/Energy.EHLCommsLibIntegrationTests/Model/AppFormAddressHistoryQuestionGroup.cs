using System;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class AppFormAddressHistoryQuestionGroup : AppFormQuestionGroup
    {
        public AppFormAddressHistoryQuestionGroup()
        {
        }

        public AppFormAddressHistoryQuestionGroup(string name, string tags) : base(name, tags)
        {

        }

        public int AddressHistoryMonthsRequired { get; set; }
    }
}
