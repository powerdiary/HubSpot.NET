using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class PagingCursor
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "after")]
        public string After { get; set; }

        [DataMember(Name = "before")]
        public string Before { get; set; }
    }
}