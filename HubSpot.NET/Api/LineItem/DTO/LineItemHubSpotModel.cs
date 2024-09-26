using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.LineItem.DTO
{
    /// <summary>
    /// Models a Line Item entity within HubSpot. Default properties are included here
    /// with the intention that you'd extend this class with properties specific to
    /// your HubSpot account.
    /// </summary>
    [DataContract]
    public class LineItemHubSpotModel : IHubSpotModel
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
        /// Indicates if the Line Item is archived.
        /// </summary>
        [DataMember(Name = "archived")]
        public bool? IsArchived { get; set; }

        /// <summary>
        /// The date the line item was last updated.
        /// </summary>
        [DataMember(Name = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The associations for this Line Item.
        /// </summary>
        [IgnoreDataMember]
        public LineItemHubSpotAssociations Associations { get; } = new LineItemHubSpotAssociations();

        /// <summary>
        /// The properties of the Line Item.
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