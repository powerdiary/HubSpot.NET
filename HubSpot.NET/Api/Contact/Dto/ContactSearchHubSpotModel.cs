using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Contact.Dto
{
    public class ContactSearchHubSpotModel<T> : IHubSpotModel where T : ContactHubSpotModel, new()
    {

        [DataMember(Name = "total")]
        public long Total { get; set; }

        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }

        [DataMember(Name = "results")]
        public IList<T> Results { get; set; } = new List<T>();

        public string RouteBasePath => "/crm/v3/objects/contacts/search";

        public bool IsNameValue => false;

        public virtual void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }
    }
}
