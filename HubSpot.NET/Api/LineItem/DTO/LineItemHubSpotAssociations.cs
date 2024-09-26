namespace HubSpot.NET.Api.LineItem.DTO
{
    /// <summary>
    /// Models associations for a Line Item, such as associated deals.
    /// </summary>
    public class LineItemHubSpotAssociations
    {
        /// <summary>
        /// Associated Deals IDs for the Line Item.
        /// </summary>
        public long[] AssociatedDeals { get; set; }

        public LineItemHubSpotAssociations()
        {
            AssociatedDeals = new long[] { };
        }
    }
}