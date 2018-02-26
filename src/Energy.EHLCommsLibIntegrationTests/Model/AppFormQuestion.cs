using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Serialization;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public enum StatementType
    {
        Text,
        Image
    }

    [Serializable]
    public class Guidance
    {
        public string Title { get; set; }
        public string Reference { get; set; }
        public string Statement { get; set; }
    }

    [Serializable]
    public class TextStatement : Statement
    {
        public TextStatement()
        {
            Type = StatementType.Text;
        }

        public string Text { get; set; }
    }
    
    [Serializable]
    public class ImageStatement : Statement
    {
        public ImageStatement()
        {
            Type = StatementType.Image;
        }
        
        public string ImageUrl { get; set; }
    }


    [XmlInclude(typeof(ImageStatement))]
    [XmlInclude(typeof(TextStatement))]
    [Serializable]
    public abstract class Statement
    {
        public StatementType Type { get; set; }
        public string Reference { get; set; }
        public string AppliesWhenMatches { get; set; }
    }


    [Serializable]
    public class ConfirmationItemControl : ConfirmationItem
    {
    }

    [XmlInclude(typeof(ConfirmationItemControl))]
    [Serializable]
    public abstract class ConfirmationItem
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Prompt { get; set; }
        public string  Data { get; set; }
        public bool Mandatory { get; set; }
    }

    [XmlInclude(typeof(CheckboxQuestion))]
    [XmlInclude(typeof(DateQuestion))]
    [XmlInclude(typeof(TextboxQuestion))]
    [XmlInclude(typeof(AddressLookupQuestion))]
    [XmlInclude(typeof(UnknownAppFormQuestion))]
    [XmlInclude(typeof(SortCodeQuestion))]
    [XmlInclude(typeof(BankAccountQuestion))]
    [XmlInclude(typeof(OptionQuestion))]
    [XmlInclude(typeof(ResidencyPeriodQuestion))]
	[Serializable]
    public abstract class AppFormQuestion
    {
        protected AppFormQuestion()
        {
            Type = AppFormQuestionType.Unknown;
            Statements = new List<Statement>();
        }

        public virtual string DataBinding
        {
            get
            {
                return string.Format("value: dynamicData.{0}.{1}.data", GroupName, FieldName);
            }
        }

        public virtual dynamic DynamicData
        {
            get
            {
                dynamic field = new ExpandoObject();
                field.data = Data;
                return field;
            }
        } 
        public object Data { get; set; }
        public string Label { get; set; }
        public string FieldName { get; set; }
        public string GroupName { get; set; }
        public bool Required { get; set; }
        public AppFormQuestionType Type { get; set; }
        public string RegularExpression { get; set; }
        public string ErrorMessage { get; set; }
        public string GuidanceMessage { get; set; }
        public bool ReadOnly { get; set; }
        public int DisplayOrder { get; set; }
        public List<Statement> Statements { get; set; }
        public bool HasServerError { get; set; }

        public  bool IsAnOptionQuestion
        {
            get { return Type == AppFormQuestionType.DropdownList || Type == AppFormQuestionType.MultiBool; }
        }

        public virtual void Initialise(string groupName, Item questionItem, SignupApiResponse response)
        {
            Data = questionItem.Data; 
            GroupName = groupName;
            FieldName = questionItem.Name;
            Required = questionItem.Mandatory;
            Label = questionItem.Prompt;
            RegularExpression = questionItem.RegularExpression;
            ReadOnly = questionItem.ReadOnly;
            ErrorMessage = GetErrorMessage(groupName, questionItem, response);
            GuidanceMessage = GetGuidanceMessage(questionItem);
        }

        private string GetGuidanceMessage(Item questionItem)
        {
            if (questionItem.Guidance == null)
            {
                return string.Empty;
            }
            
            var guidence = questionItem.Guidance.FirstOrDefault();
            
            if (guidence == null)
            {
                return string.Empty;
            }

            return guidence.StatementText??string.Empty;
        }

        protected string GetErrorMessage(string groupName, Item questionItem, SignupApiResponse response)
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(questionItem.RegularExpressionErrorMessage))
            {
                errorMessage =  questionItem.RegularExpressionErrorMessage;
            }

            if (response.Errors == null) return errorMessage;

            var error = response.Errors.FirstOrDefault(
                a =>
                {
                    if (a.Item == null || a.Group == null)
                    {
                        return false;
                    }
                    
                    return a.Group.Equals(groupName) && a.Item.Equals(questionItem.Name);
                    
                });

            if (error == null)
            {
                HasServerError = false;
                return errorMessage;
            }

            HasServerError = true;

            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = error.Message.Text;
            }
            return errorMessage;
        }

        protected string GetLinkedUrl(string validateAs, SignupApiResponse response)
        {
            if (response.LinkedDataSources == null)
            {
                return string.Empty;
            }

            foreach (var linkedDataSource in response.LinkedDataSources)
            {
                if (linkedDataSource.Queries != null && linkedDataSource.Queries.Any(q => q.Parameters.Any(p => p.ValidateAs.Equals(validateAs))))
                    return linkedDataSource.Uri;
            }
            return string.Empty;
        }

    }
}