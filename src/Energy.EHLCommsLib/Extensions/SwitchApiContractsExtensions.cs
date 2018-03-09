using System.Linq;
using Energy.EHLCommsLib.Contracts.Common;
using Energy.EHLCommsLib.Contracts.Common.Data;
using Energy.EHLCommsLib.Contracts.Responses;

namespace Energy.EHLCommsLib.Extensions
{
    public static class SwitchApiContractsExtensions
    {
        private const string NextRel = "/rels/next";

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