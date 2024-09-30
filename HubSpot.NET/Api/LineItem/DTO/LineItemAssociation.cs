using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class LineItemAssociation
    {
        /// <summary>
        /// The target object for the association.
        /// </summary>
        [DataMember(Name = "to")]
        public LineItemAssociationTarget To { get; set; }

        /// <summary>
        /// The types of associations.
        /// </summary>
        [DataMember(Name = "types")]
        public List<LineItemAssociationType> Types { get; set; }

        public LineItemAssociation()
        {
            To = new LineItemAssociationTarget();
            Types = new List<LineItemAssociationType>();
        }
    }
}