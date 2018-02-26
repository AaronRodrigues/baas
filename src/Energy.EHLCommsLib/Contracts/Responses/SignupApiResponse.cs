using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Signup;

namespace Energy.EHLCommsLib.Contracts.Responses

{
    [DataContract]
    public class SignupApiResponse : ApiResponse
    {
        public SignupApiResponse()
        {
            Links = new List<Link>();
        }

        [DataMember(Name = "questions", EmitDefaultValue = false)]
        public List<QuestionGroupStatus> QuestionStatuses { get; set; }

        [DataMember(Name = "supplyAddressSearchResults", EmitDefaultValue = false)]
        public List<AddressSearchResult> SupplyAddressSearchResults { get; set; }

        [DataMember(Name = "supplierPreferredPaymentDays", EmitDefaultValue = false)]
        public List<int> SupplierPreferredPaymentDays { get; set; }

        [DataMember(Name = "addressHistoryMonthsRequired", EmitDefaultValue = false)]
        public int? AddressHistoryMonthsRequired { get; set; }
    }
}