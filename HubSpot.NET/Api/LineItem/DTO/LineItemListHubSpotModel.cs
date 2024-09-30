using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.LineItem.DTO
{
    /// <summary>
    /// Models a set of results returned when querying for sets of line items.
    /// </summary>
    [DataContract]
    public class LineItemListHubSpotModel<T> : IHubSpotModel where T : LineItemGetResponse, new()
    {
        [DataMember(Name = "results")]
        public IList<T> LineItems { get; set; } = new List<T>();

        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; } = new PagingModel();

        public string RouteBasePath => "/crm/v3/objects/line_items";

        public bool IsNameValue => false;

        public virtual void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
            // No additional implementation for now
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
            if (hubspotData.results != null)
            {
                LineItems = ((List<object>)hubspotData.results).Cast<T>().ToList();
            }

            if (hubspotData.paging != null)
            {
                Paging = new PagingModel
                {
                    Next = hubspotData.paging.next
                };
            }
        }
    }
}