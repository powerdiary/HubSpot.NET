using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Properties.Dto;

[DataContract]
public class PropertiesListHubSpotModel<T> : IHubSpotModel where T : IHubSpotModel
{
    [DataMember(Name = "results")] public List<T> Results { get; set; }

    public string RouteBasePath
    {
        get
        {
            var entity = (T)Activator.CreateInstance(typeof(T));
            return "crm/v3/properties" + entity.RouteBasePath;
        }
    }

    public bool IsNameValue => false;

    public virtual void ToHubSpotDataEntity(ref dynamic dataEntity)
    {
    }

    public virtual void FromHubSpotDataEntity(dynamic hubspotData)
    {
    }
}