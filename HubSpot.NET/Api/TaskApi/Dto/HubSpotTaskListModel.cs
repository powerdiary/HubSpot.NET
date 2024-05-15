using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.TaskApi.Dto;

public class HubSpotTaskListModel<T> : IHubSpotModel where T : HubSpotTaskModel, new()
{
    [DataMember(Name = "paging")] public PagingModel Paging { get; set; }

    /// <summary>
    /// Gets or sets the <typeparamref name="T"/>.
    /// </summary>
    /// <value>
    /// The <typeparamref name="T"/>.
    /// </value>
    [DataMember(Name = "results")]
    public IList<T> Results { get; set; } = new List<T>();

    public string RouteBasePath => "/crm/v3/objects/tasks";

    public bool IsNameValue => false;

    public virtual void ToHubSpotDataEntity(ref dynamic dataEntry)
    {
    }

    public virtual void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }
}