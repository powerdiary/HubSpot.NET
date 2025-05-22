using HubSpot.NET.Api.Schemas;
using HubSpot.NET.Core.Interfaces;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.CustomEvent.Dto
{
    [DataContract]
    public class EventDefinition : IHubSpotModel
    {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "labels")]
        public SchemasLabelsModel Labels { get; set; }
        
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "primaryObjectId")]
        public string PrimaryObjectId { get; set; }

        [DataMember(Name = "fullyQualifiedName")]
        public string FullyQualifiedName { get; set; }

        [DataMember(Name = "archived")]
        public bool Archived { get; set; }

        [DataMember(Name = "trackingType")]
        public string TrackingType { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "createdAt")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public string UpdatedAt { get; set; }

        public bool IsNameValue => true;

        public string RouteBasePath => "/events/v3/event-definitions";
       
        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
            
        }

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
            
        }
    }    
}
