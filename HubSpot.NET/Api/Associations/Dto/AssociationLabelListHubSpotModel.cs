using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationLabelListHubSpotModel : IHubSpotModel
    {
        [JsonProperty(PropertyName = "results")]
        public List<AssociationLabel> Results { get; set; } = new List<AssociationLabel>();

        public class AssociationLabel
        {
            [JsonProperty(PropertyName = "category")]
            public string Category { get; set; }

            [JsonProperty(PropertyName = "typeId")]
            public int TypeId { get; set; }

            [JsonProperty(PropertyName = "label")]
            public string Label { get; set; }
        }

        public bool IsNameValue => false;

        public void ToHubSpotDataEntity(ref dynamic dataEntity) { }

        public void FromHubSpotDataEntity(dynamic hubspotData) { }

        public string RouteBasePath => "/crm/v4/associations/";
    }
}
