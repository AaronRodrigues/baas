using System;
using System.Text.RegularExpressions;
using Energy.EHLCommsLib.Enums;

namespace Energy.EHLCommsLib.Extensions
{
    public static class ExtensionMethods
    {
        public static string GetFeatureClassName(this PriceFeatureCategory featureCategory)
        {
            switch (featureCategory)
            {
                case PriceFeatureCategory.CappedOrFixed:
                    return "capped-fixed";
                case PriceFeatureCategory.SpecialOffers:
                    return "special-offers";
                default:
                    return string.Empty;
            }
        }

        public static decimal RoundToNearest(this decimal val, decimal scale)
        {
            var multipliedValue = (val*100M/scale);

            var result = Math.Round(multipliedValue)*scale;

            return result/100;
        }

        public static string RemoveHtmlTags(this string input)
        {
            return input == null ? null : Regex.Replace(input, @"<[^>]*>", String.Empty);
        }

        public static string GetOriginalMessage(this Exception exception)
        {
            var originalException = exception.InnerException ?? exception;
            return originalException.Message;
        }

        public static string GetOriginalStack(this Exception exception)
        {
            var originalException = exception.InnerException ?? exception;
            return originalException.StackTrace;
        }

        public static AppFormQuestionType ConvertToAppFormQuestionType(this string type)
        {
            switch (type)
            {
                case "oneOf":
                case "linked":
                    return AppFormQuestionType.DropdownList;
                case "anyOf":
                    return AppFormQuestionType.MultiBool;
                case "text":
                case "numericString":
                    return AppFormQuestionType.Text;
                case "bool":
                    return AppFormQuestionType.Bool;
                case "date":
                    return AppFormQuestionType.Date;
                case "password":
                case "maskedString":
                    return AppFormQuestionType.Password;
                default:
                    return AppFormQuestionType.Unknown;
            }
        }

        public static string EhlSupplyType(this CompareWhat compareWhat)
        {
            switch (compareWhat)
            {
                case CompareWhat.Both:
                    return "4";
                case CompareWhat.Electricity:
                    return "2";
                default:
                    return "1";
            }
        }
    }
}