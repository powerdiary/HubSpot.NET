using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class LineItemCreateOrUpdateRequest
    {
        private const int DealAssociationTypeId = 20;  // Assuming 20 is the associationTypeId for deals

        /// <summary>
        /// Line Item's unique ID in HubSpot. This is nullable for create operations.
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }  // Nullable Id for create and update

        /// <summary>
        /// The properties of the Line Item.
        /// Reuses the shared properties model.
        /// </summary>
        [DataMember(Name = "properties")]
        public LineItemPropertiesHubSpotModel Properties { get; set; } = new LineItemPropertiesHubSpotModel();

        /// <summary>
        /// A list of associations to include when creating line items (e.g., deals).
        /// </summary>
        [DataMember(Name = "associations")]
        public List<LineItemAssociation> Associations { get; set; } = new List<LineItemAssociation>();

        /// <summary>
        /// Convenience property for associating deals (sugar syntax).
        /// This is ignored in serialization, but can be used to easily set associated deals.
        /// </summary>
        [IgnoreDataMember]
        public long[] AssociatedDeals
        {
            get => GetAssociatedDealIds();
            set
            {
                Associations.RemoveAll(a => a.Types.Exists(t => t.AssociationTypeId == DealAssociationTypeId));

                if (value != null && value.Any())
                {
                    foreach (var dealId in value)
                    {
                        Associations.Add(new LineItemAssociation
                        {
                            To = new LineItemAssociationTarget { Id = dealId },
                            Types = new List<LineItemAssociationType>
                            {
                                new LineItemAssociationType
                                {
                                    AssociationCategory = "HUBSPOT_DEFINED",
                                    AssociationTypeId = DealAssociationTypeId
                                }
                            }
                        });
                    }
                }
            }
        }

        private long[] GetAssociatedDealIds()
        {
            return (from a in Associations
                where a.Types.Exists(t => t.AssociationTypeId == DealAssociationTypeId)
                select a.To.Id).ToArray();
        }
    }
}