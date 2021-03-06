﻿using System;
using System.Collections.Generic;
using System.Linq;
using Energy.EHLCommsLib.Contracts;
using Energy.EHLCommsLib.Enums;
using Energy.EHLCommsLib.Models.Prices;

namespace Energy.EHLCommsLib.Extensions
{
    public static class TariffFeaturesExtensions
    {
        public static List<PriceFeature> ToPriceFeatures(this IEnumerable<TagTextPair> keyFeatures)
        {
            var featureList = new List<PriceFeature>();

            foreach (var feature in keyFeatures)
            {
                // Ensure that the tags are trimmed and delimited by a single space - guard against double space typo's
                var tags = feature.Tags.Trim().Replace("  ", " ").Split(' ').ToList();
                featureList.AddRange(
                    tags.Select(
                        tag =>
                            new PriceFeature
                            {
                                Category = GetPriceFeatureCategory(tag),
                                Description = RemoveHtmlTags(feature.Text)
                            }));
            }

            return featureList;
        }

        private static PriceFeatureCategory GetPriceFeatureCategory(string tags)
        {
            if (!Enum.IsDefined(typeof (PriceFeatureCategory), tags))
            {
                return PriceFeatureCategory.Other;
            }

            return (PriceFeatureCategory) Enum.Parse(typeof (PriceFeatureCategory), tags);
        }

        private static string RemoveHtmlTags(string input)
        {
            // taken from article - char array chosen over regex for performance  http://www.dotnetperls.com/remove-html-tags
            var array = new char[input.Length];
            var arrayIndex = 0;
            var inside = false;

            foreach (var @let in input)
            {
                if (@let == '<')
                {
                    inside = true;
                    continue;
                }
                if (@let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = @let;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }
    }
}