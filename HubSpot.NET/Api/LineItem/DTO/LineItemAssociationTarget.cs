using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class LineItemAssociationTarget
    {
        /// <summary>
        /// The ID of the object to associate with.
        /// </summary>
        [DataMember(Name = "id")]
        public long Id { get; set; }
    }
}