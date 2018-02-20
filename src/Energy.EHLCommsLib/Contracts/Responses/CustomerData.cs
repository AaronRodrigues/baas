using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Energy.EHLCommsLib.Contracts.Responses
{
    [DataContract]
    public class CustomerData
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "partnerReference")]
        public string PartnerReference { get; set; }

        [DataMember(Name = "EHLReference")]
        public string EHLReference { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "surname")]
        public string Surname { get; set; }

        [DataMember(Name = "eMail")]
        public string EMail { get; set; }

        [DataMember(Name = "supplyAddress")]
        public string SupplyAddress { get; set; }

        [DataMember(Name = "daytimePhoneNumber")]
        public string DaytimePhoneNumber { get; set; }

        [DataMember(Name = "contactByEMailAllowed")]
        public bool ContactByEMailAllowed { get; set; }

        [DataMember(Name = "contactByPostAllowed")]
        public bool ContactByPostAllowed { get; set; }

        [DataMember(Name = "contactByTelephoneAllowed")]
        public bool ContactByTelephoneAllowed { get; set; }

        [DataMember(Name = "contactBySMSAllowed")]
        public bool ContactBySMSAllowed { get; set; }

        [DataMember(Name = "nextSteps")]
        public string NextSteps { get; set; }

        [DataMember(Name = "eveningPhoneNumber")]
        public string EveningPhoneNumber { get; set; }

        [DataMember(Name = "dateOfBirth")]
        public string DateOfBirth { get; set; }

        [DataMember(Name = "applicationDate")]
        public string ApplicationDate { get; set; }

        [DataMember(Name = "correspondenceAddress")]
        public string CorrespondenceAddress { get; set; }

        [DataMember(Name = "isSwitchHeld")]
        public string IsSwitchHeld { get; set; }

        [DataMember(Name = "heldUntilDate", EmitDefaultValue = false)]
        public string HeldUntilDate { get; set; }
    }
}
