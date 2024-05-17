using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;

namespace HubSpot.NET.Api.CustomObject
{
    [DataContract]
    public class CreateCustomObjectHubSpotModel : IHubSpotModel
    {
        [IgnoreDataMember] public string SchemaId { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        public bool IsNameValue => false;

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public string RouteBasePath => "crm/v3/objects";

        [JsonProperty(PropertyName = "associations")]
        public List<Association> Associations { get; set; } = new List<Association>();

        public class Association
        {
            public To To { get; set; }
            public List<TypeElement> Types { get; set; }
        }

        public class To
        {
            public string Id { get; set; }
        }

        public class TypeElement
        {
            // either HUBSPOT_DEFINED, USER_DEFINED, INTEGRATOR_DEFINED
            public string AssociationCategory { get; set; }
            public long? AssociationTypeId { get; set; }
        }
    }
}