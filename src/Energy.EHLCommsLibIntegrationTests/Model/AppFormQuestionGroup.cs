using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [XmlInclude(typeof(AppFormAddressHistoryQuestionGroup))]
    [XmlInclude(typeof(AppFormOverseasAddressHistoryQuestionGroup))]
    [Serializable]
    public class AppFormQuestionGroup
    {
        protected AppFormQuestionGroup()
        {
            Questions = new List<AppFormQuestion>();
            Statements = new List<Statement>();
            Guidance = new List<Guidance>();
        }

        public AppFormQuestionGroup(string name, string tags) : this()
        {
            Name = name;
            Tags = tags;
        }

        public string Name { get; set; }
        public string Tags { get; set; }

        public List<AppFormQuestion> Questions { get; set; }
        public List<Statement> Statements { get; set; }
        public List<Guidance> Guidance { get; set; }
    }

}