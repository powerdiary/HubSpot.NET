using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class AssociationResult
    {
        /// <summary>
        /// ID of the associated object (e.g., Deal).
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// Type of the association (e.g., line_item_to_deal).
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}