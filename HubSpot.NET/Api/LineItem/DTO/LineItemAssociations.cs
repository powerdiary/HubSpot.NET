using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class LineItemAssociations
    {
        /// <summary>
        /// Deals associated with this Line Item.
        /// </summary>
        [DataMember(Name = "deals")]
        public AssociationList Deals { get; set; }
    }
}