using System;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class BankAccountQuestion : AppFormQuestion
    {
        public BankAccountQuestion()
        {
            Type = AppFormQuestionType.BankAccount;
        }

        public string ValidationUrl { get; set; }

        public override void Initialise(string groupName, Item questionItem, SignupApiResponse response)
        {
            base.Initialise(groupName, questionItem, response);
            ValidationUrl = GetLinkedUrl(questionItem.ValidateAs, response);
        }
    }
}