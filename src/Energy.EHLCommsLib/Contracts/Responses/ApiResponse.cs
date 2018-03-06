using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Energy.EHLCommsLib.Constants;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.Responses
{
    [DataContract]
    [KnownType(typeof (CurrentSupplies))]
    public class ApiResponse
    {
        [DataMember(Name = "links", EmitDefaultValue = false)]
        public List<Link> Links { get; set; }

        [DataMember(Name = "linked-data", EmitDefaultValue = false)]
        public List<LinkedDataSource> LinkedDataSources { get; set; }

        [DataMember(Name = "data-template", EmitDefaultValue = false)]
        public DataTemplate DataTemplate { get; set; }

        [DataMember(Name = "errors", EmitDefaultValue = false)]
        public List<Error> Errors { get; set; }

        [XmlIgnore]
        public Exception Exception { get; set; }

        public HttpStatusCode? StatusCode { get; set; }
        public List<KeyValuePair<string, string>> HttpHeaders { get; set; }
        public string MediaType { get; set; }

        public bool IsExpectedError()
        {
            if (EhlErrorConstants.ExpectedErrors.Any(HasErrorMessageId))
            {
                return true;
            }

            return false;
        }

        public bool HasErrorMessageId(string messageId)
        {
            return Errors != null && Errors.Any(o => o.Message != null && o.Message.Id == messageId);
        }

        public bool SuccessfulResponseFromEhl()
        {
            return (Errors == null || Errors.Count == 0) && StatusCode != HttpStatusCode.InternalServerError;
        }
    }
}