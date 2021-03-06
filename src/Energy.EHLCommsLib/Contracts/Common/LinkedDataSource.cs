﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLib.Contracts.Common
{
    [DataContract]
    public class LinkedDataSource
    {
        [DataMember(Name = "rel")]
        public string Rel { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "media", EmitDefaultValue = false)]
        public string MediaType { get; set; }

        [DataMember(Name = "queries", EmitDefaultValue = false)]
        public List<Query> Queries { get; set; }
    }
}