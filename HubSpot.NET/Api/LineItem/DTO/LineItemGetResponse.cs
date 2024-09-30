using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class LineItemGetResponse : IHubSpotModel
    {
        /// <summary>
        /// Line Item's unique ID in HubSpot.
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// The date the line item was created.
        /// </summary>
        [DataMember(Name = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The date the line item was last updated.
        /// </summary>
        [DataMember(Name = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Indicates if the Line Item is archived.
        /// </summary>
        [DataMember(Name = "archived")]
        public bool? IsArchived { get; set; }

        /// <summary>
        /// The associations of the Line Item, such as associated deals.
        /// </summary>
        [DataMember(Name = "associations")]
        public LineItemAssociations Associations { get; set; }

        /// <summary>
        /// The properties of the Line Item.
        /// Reuses the shared properties model.
        /// </summary>
        [DataMember(Name = "properties")]
        public LineItemPropertiesHubSpotModel Properties { get; set; } = new LineItemPropertiesHubSpotModel();

        public string RouteBasePath => "/crm/v3/objects/line_items";
        public bool IsNameValue => true;

        public virtual void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
            dataEntity.Associations = Associations;
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
            throw new NotSupportedException("FromHubSpotDataEntity is not supported in this class.");
        }
    }
}