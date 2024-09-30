using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class LineItemAssociationType
    {
        /// <summary>
        /// The category of the association, usually "HUBSPOT_DEFINED".
        /// </summary>
        [DataMember(Name = "associationCategory")]
        public string AssociationCategory { get; set; }

        /// <summary>
        /// The association type ID, which defines the specific relationship type (e.g., deals to line items).
        /// </summary>
        [DataMember(Name = "associationTypeId")]
        public int AssociationTypeId { get; set; }
    }
}