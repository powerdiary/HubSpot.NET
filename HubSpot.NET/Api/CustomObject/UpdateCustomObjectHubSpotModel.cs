using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;

namespace HubSpot.NET.Api.CustomObject;

[DataContract]
public class UpdateCustomObjectHubSpotModel : IHubSpotModel
{
    [DataMember(Name = "id")] public string Id { get; set; }

    [IgnoreDataMember] public string SchemaId { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    public bool IsNameValue => false;

    public void ToHubSpotDataEntity(ref dynamic dataEntity)
    {
    }

    public void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }

    public string RouteBasePath => "crm/v3/objects";
}