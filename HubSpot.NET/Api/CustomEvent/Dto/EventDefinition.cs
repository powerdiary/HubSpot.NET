using HubSpot.NET.Api.Schemas;
using HubSpot.NET.Core.Interfaces;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.CustomEvent.Dto
{
    [DataContract]
    public class EventDefinition : IHubSpotModel
    {
        [DataMember(Name = "labels")]
        public SchemasLabelsModel Label { get; set; }
        
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "primaryObject")]
        public string PrimaryObject { get; set; }

        [DataMember(Name = "fullyQualifiedName")]
        public string FullyQualifiedName { get; set; }

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
