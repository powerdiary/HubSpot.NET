using System.Collections.Generic;
using HubSpot.NET.Core;

namespace HubSpot.NET.Api.LineItem
{
    /// <summary>
    /// Options for requesting lists of line items from HubSpot.
    /// </summary>
    public class LineItemListRequestOptions : ListRequestOptions
    {
        /// <summary>
        /// A list of associations to include when retrieving line items.
        /// </summary>
        public List<string> Associations { get; set; } = new List<string>();

        /// <summary>
        /// Additional properties to include in the request (optional).
        /// This could be used to request specific line item fields.
        /// </summary>
        public List<string> PropertiesToInclude { get; set; } = new List<string>();
    }
}