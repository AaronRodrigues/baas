using System.Linq;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;

namespace Energy.EHLCommsLib.Extensions
{
    public static class SwitchApiContractsExtensions
    {
        private const string NextRel = "/rels/next";

        public static Item GetEhlItemForName(this ApiResponse apiResponse, string name)
        {
            Item item = null;
            foreach (var group in apiResponse.DataTemplate.Groups)
            {
                item = group.Items.FirstOrDefault(i => i.Name.Equals(name));
                if (item != null)
                    break;
            }
            return item;
        }

        public static Item GetItem(this DataTemplate template, string groupName, string itemName)
        {
            var group = template.Groups.FirstOrDefault(g => g.Name.Equals(groupName));
            return @group?.Items.FirstOrDefault(i => i.Name.Equals(itemName));
        }

        public static Link GetExactLinkForRel(this ApiResponse response, string rel)
        {
            return response.Links?.FirstOrDefault(l => l.Rel.Equals(rel));
        }

        public static Link GetLinkForRel(this ApiResponse response, string rel)
        {
            return response.Links?.FirstOrDefault(l => l.Rel.Contains(rel));
        }

        public static string GetNextRelUrl(this ApiResponse response, string nextRel)
        {
            return response.Links
                .First(l => l.Rel.Contains(nextRel) && l.Rel.Contains("/rels/next"))
                .Uri;
        }

        public static Link NextStep(this ApiResponse apiResponse)
        {
            return apiResponse.Links.SingleOrDefault(l => l.Rel.Contains(NextRel));
        }

        public static bool Is(this Link relLink, string nextRelKey)
        {
            return relLink != null && relLink.Rel.Contains(nextRelKey);
        }

        public static void UpdateItemData(this ApiResponse currentSupplyTemplate, string groupName, string itemName,
            string value)
        {
            currentSupplyTemplate.DataTemplate.Groups
                .First(g => g.Name.Equals(groupName))
                .Items.First(i => i.Name.Equals(itemName))
                .Data = value;
        }

        public static string GetLinkedDataUrl(this ApiResponse response, string rel)
        {
            return response.LinkedDataSources
                .First(l => l.Rel.Equals(rel))
                .Uri;
        }
    }
}