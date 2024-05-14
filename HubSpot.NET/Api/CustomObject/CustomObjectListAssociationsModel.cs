using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.CustomObject;

public sealed class CustomObjectListAssociationsModel<T> : IHubSpotModel where T : CustomObjectAssociationModel, new()
{
    [DataMember(Name = "results")] public IList<T> Results { get; set; } = new List<T>();
    public bool IsNameValue => false;

    public string RouteBasePath => "crm/v3/objects";

    public void ToHubSpotDataEntity(ref dynamic dataEntity)
    {
    }

    public void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }
}