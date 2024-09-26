using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class PagingInfo
    {
        /// <summary>
        /// Information about the next page.
        /// </summary>
        [DataMember(Name = "next")]
        public PagingCursor Next { get; set; }

        /// <summary>
        /// Information about the previous page.
        /// </summary>
        [DataMember(Name = "prev")]
        public PagingCursor Previous { get; set; }
    }
}