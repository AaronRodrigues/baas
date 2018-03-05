using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [XmlInclude(typeof(DropdownQuestion))]
    [XmlInclude(typeof(MultiCheckboxQuestion))]
    [Serializable]
    public abstract class OptionQuestion : AppFormQuestion
    {
        protected OptionQuestion()
        {
            AcceptableItems = new List<OptionItem>();
        }
        
        public List<OptionItem> AcceptableItems { get; set; }

        public override void Initialise(string groupName, Item questionItem, SignupApiResponse response)
        {
            base.Initialise(groupName, questionItem, response);

            AcceptableItems = MapAcceptableValues(questionItem);
        }
        
        protected List<OptionItem> MapAcceptableValues(Item ehlQuestionItem)
        {
            return ehlQuestionItem.AcceptableValues?.Select(a => new OptionItem { Id = a.Id, Name = a.Name }).ToList() ?? new List<OptionItem>();
        }
    }
}