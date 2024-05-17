using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;

namespace HubSpot.NET.Api.CustomObject
{
    [DataContract]
    public class CustomObjectHubSpotModel : IHubSpotModel
    {
        [DataMember(Name = "id")] public string Id { get; set; }

        [DataMember(Name = "createdAt")] public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")] public DateTime? UpdatedAt { get; set; }

        [IgnoreDataMember]
        [JsonProperty(PropertyName = "properties")]
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public bool IsNameValue => false;

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public string RouteBasePath => "crm/v3/objects";
    }
}