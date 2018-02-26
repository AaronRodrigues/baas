using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.EHLCommsLibIntegrationTests.Model;

namespace Energy.EHLCommsLibIntegrationTests.Helpers
{
    public class ApplicationFormDataBuilder
    {
        private readonly ApplicationFormData _applicationFormData = new ApplicationFormData
        {
            QuestionGroups = new List<AppFormQuestionGroup>()
        };

        public ApplicationFormDataBuilder WithHeaderData(Guid switchId, int supplierId, string supplierName, string tariffName, string paymentName, decimal annualSaving, decimal annualSpend, decimal monthlySpend, int supplierRating, string tariffTypeDescription, string journeyType = "HaveMyBill", string entryPointUrl = "/energy")
        {
            _applicationFormData.SwitchId = switchId;
            _applicationFormData.SupplierId = supplierId;
            _applicationFormData.SupplierName = supplierName;
            _applicationFormData.TariffName = tariffName;
            _applicationFormData.PaymentMethodName = paymentName;
            _applicationFormData.AnnualSavings = annualSaving;
            _applicationFormData.MonthlySpend = monthlySpend;
            _applicationFormData.SupplierRating = supplierRating;
            _applicationFormData.TariffTypeDescription = tariffTypeDescription;
            _applicationFormData.AnnualSpend = annualSpend;
            _applicationFormData.JourneyType = journeyType;
            _applicationFormData.EntryPointUrl = entryPointUrl;

            return this;
        }

        public ApplicationFormDataBuilder WithAddressHistoryGroup()
        {
            WithDropDownQuestion("addresshistorytwopreviousaddresses", "timeAtSupplyAddress", true, new List<OptionItem>())
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousPostcode", true)
            .WithDropDownQuestion("addresshistorytwopreviousaddresses", "knownfirstPreviousAddress", true, new List<OptionItem>())
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousFlatNumber", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousHouseNumber", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousHouseName", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousAddressLine1", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousAddressLine2", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousTown", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "firstPreviousCounty", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "timeAtFirstPreviousAddress", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousPostcode", true)
            .WithDropDownQuestion("addresshistorytwopreviousaddresses", "knownsecondPreviousAddress", true, new List<OptionItem>())
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousFlatNumber", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousHouseNumber", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousHouseName", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousAddressLine1", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousAddressLine2", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousTown", true)
            .WithTextboxQuestion("addresshistorytwopreviousaddresses", "secondPreviousCounty", true);

            return this;
        }

        public ApplicationFormDataBuilder WithOverseasAddressHistoryGroup()
        {
            WithDropDownQuestion("addresshistorywithoverseasoption", "timeAtSupplyAddress", true, new List<OptionItem>())
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousPostcode", true)
                .WithDropDownQuestion("addresshistorywithoverseasoption", "knownfirstPreviousAddress", true,
                                      new List<OptionItem>())
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousFlatNumber", true)
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousHouseNumber", true)
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousHouseName", true)
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousAddressLine1", true)
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousAddressLine2", true)
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousTown", true)
                .WithTextboxQuestion("addresshistorywithoverseasoption", "firstPreviousCounty", true)
                .WithCheckBoxQuestion("addresshistorywithoverseasoption", "firstpreviousaddresswasoverseas");

            return this;
        }

        public ApplicationFormDataBuilder WithTextboxQuestion(string groupName, string fieldName, bool required, string errorMessage = "")
        {
            var group = GetGroup(groupName);

            group.Questions.Add(new TextboxQuestion
            {
                GroupName = groupName,
                FieldName = fieldName,
                Required = required,
                ErrorMessage = errorMessage
            });

            if (!string.IsNullOrEmpty(errorMessage))
            {
                _applicationFormData.HasErrors = true;
            }

            return this;
        }

        public ApplicationFormDataBuilder WithTextStatement(string groupName, string reference, string text)
        {
            var group = GetGroup(groupName);

            var statement = new TextStatement
            {
                Reference = reference,
                Text = text,
            };

            group.Statements.Add(statement);

            return this;
        }

        public ApplicationFormDataBuilder WithImageStatement(string groupName, string reference, string imageUrl)
        {
            var group = GetGroup(groupName);

            var statement = new ImageStatement
            {
                Reference = reference,
                ImageUrl = imageUrl
            };

            group.Statements.Add(statement);

            return this;
        }

        public ApplicationFormDataBuilder WithDropDownQuestion(string groupName, string fieldName, bool required, List<OptionItem> items)
        {
            var group = GetGroup(groupName);

            group.Questions.Add(new DropdownQuestion
            {
                GroupName = groupName,
                FieldName = fieldName,
                Required = required,
                AcceptableItems = items
            });

            return this;
        }

        public ApplicationFormDataBuilder WithDateQuestion(string groupName, string fieldName, bool required, string date)
        {
            var group = GetGroup(groupName);

            group.Questions.Add(new DateQuestion
            {
                Data = date,
                FieldName = fieldName,
                GroupName = groupName,
                Required = true
            });

            return this;
        }

        private AppFormQuestionGroup GetGroup(string groupName)
        {
            var group = _applicationFormData.QuestionGroups.FirstOrDefault(g => g.Name.Equals(groupName));
            if (group == null)
            {
                group = new AppFormQuestionGroup(groupName, groupName);
                _applicationFormData.QuestionGroups.Add(group);
            }
            return group;
        }

        public ApplicationFormData Build()
        {
            return _applicationFormData;
        }

        public ApplicationFormDataBuilder WithCheckBoxQuestion(string groupName, string fieldName)
        {
            var group = GetGroup(groupName);

            group.Questions.Add(new TextboxQuestion
            {
                GroupName = groupName,
                FieldName = fieldName
            });

            return this;
        }

        public ApplicationFormDataBuilder WithDynamicDropDownQuestion(string groupName, string fieldName)
        {
            var group = GetGroup(groupName);

            group.Questions.Add(new DropdownQuestion
            {
                GroupName = groupName,
                FieldName = fieldName,
                Dynamic = true
            });

            return this;
        }

        public ApplicationFormDataBuilder WithSortCodeQuestion(string groupName, string fieldName, string validationUrl)
        {
            var group = GetGroup(groupName);

            group.Questions.Add(new SortCodeQuestion
            {
                GroupName = groupName,
                FieldName = fieldName,
                ValidationUrl = validationUrl,
            });

            return this;
        }

        public ApplicationFormDataBuilder WithBankAccountQuestion(string groupName, string fieldName, string validationUrl)
        {
            var group = GetGroup(groupName);

            group.Questions.Add(new BankAccountQuestion
            {
                GroupName = groupName,
                FieldName = fieldName,
                ValidationUrl = validationUrl,
            });

            return this;
        }
    }
}