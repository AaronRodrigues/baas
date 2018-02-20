using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return group == null ? null : group.Items.FirstOrDefault(i => i.Name.Equals(itemName));
        }

        public static Link GetExactLinkForRel(this ApiResponse response, string rel)
        {
            return response.Links != null
                       ? response.Links.FirstOrDefault(l => l.Rel.Equals(rel))
                       : null;
        }

        public static Link GetLinkForRel(this ApiResponse response, string rel)
        {
            return response.Links != null
                       ? response.Links.FirstOrDefault(l => l.Rel.Contains(rel))
                       : null;
        }

        public static string GetNextRelUrl(this ApiResponse response, string nextRel)
        {
            return response.Links
                .First(l => l.Rel.Contains(nextRel) && l.Rel.Contains("/rels/next"))
                .Uri;
        }

        public static Link NextStep(this SwitchesApiResponse apiResponse)
        {
            return apiResponse.Links.SingleOrDefault(l => l.Rel.Contains(NextRel));
        }

        public static bool Is(this Link relLink, string nextRelKey)
        {
            return relLink != null && relLink.Rel.Contains(nextRelKey);
        }
    }
}
