using HubSpot.NET.Api.CustomEvent.Dto;
using System.Threading.Tasks;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotCustomEventApi
    {
        Task SendEventTrackingData(EventTracking eventTracking);        

        Task<T> GetByNameAsync<T>(string eventName) where T : EventDefinition, new();
    }
}
