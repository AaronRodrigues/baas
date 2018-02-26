using System;
using System.Collections.Generic;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class ApplicationFormData
    {
        public Guid SwitchId { get; set; }

        public string SignupUrl { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string TariffName { get; set; }
        public string PaymentMethodName { get; set; }
        public List<PriceFeature> KeyFeatures { get; set; }
        public decimal AnnualSavings { get; set; }
        public decimal AnnualSpend { get; set; }
        public string CappedOrFixedFeatureText { get; set; }
        public bool TimedOut { get; set; }
        public string Postcode { get; set; }
        public List<AppFormQuestionGroup> QuestionGroups { get; set; }
        public bool HasErrors { get; set; }
        public string JourneyType { get; set; }
        public string EntryPointUrl { get; set; }
        public string CompareWhat { get; set; }
        public string ResultsUrl { get; set; }
        public string ConfirmationUrl { get; set; }
        public bool AlreadySwitched { get; set; }
        public string SupplierTermsAndConditionsUrl { get; set; }
        public string SupplierTermsAndConditionsType { get; set; }
        public List<Statement> ConfirmationStatements { get; set; }
        public List<ConfirmationItem> ConfirmationControls { get; set; }
        public decimal MonthlySpend { get; set; }
        public string TariffTypeDescription { get; set; }
        public int SupplierRating { get; set; }
    }
}