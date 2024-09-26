using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class AssociationList
    {
        /// <summary>
        /// Paging information for associations.
        /// </summary>
        [DataMember(Name = "paging")]
        public PagingInfo Paging { get; set; }

        /// <summary>
        /// List of results for associations.
        /// </summary>
        [DataMember(Name = "results")]
        public List<AssociationResult> Results { get; set; }
    }
}