using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Deal.Dto
{
    /// <summary>
    /// Models a set of results returned when querying for sets of deals
    /// </summary>
    [DataContract]
    public class DealListHubSpotModel<T> : IHubSpotModel where T: DealHubSpotModel, new()
    {
        [DataMember(Name = "results")]
        public IList<T> Deals { get; set; } = new List<T>();

        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; } = new PagingModel();

        public string RouteBasePath => "/crm/v3/objects";

        public bool IsNameValue => false;
        public virtual void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }
    }
}
