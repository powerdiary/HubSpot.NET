using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.CustomObject;

public class CustomObjectListHubSpotModel<T> : IHubSpotModel where T: CustomObjectHubSpotModel, new()
{
    [DataMember(Name = "results")]
    public IList<T> Results { get; set; } = new List<T>();
    public bool IsNameValue => false;

    public string RouteBasePath => "crm/v3/objects";
    public virtual void ToHubSpotDataEntity(ref dynamic dataEntity)
    {
    }

    public virtual void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }
}