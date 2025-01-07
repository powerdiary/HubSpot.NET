using HubSpot.NET.Api.CustomEvent.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;
using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace HubSpot.NET.Api.CustomEvent
{
    public class HubSpotCustomEventApi : IHubSpotCustomEventApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotCustomEventApi(IHubSpotClient  client)
        {
            _client = client;
        }

        public Task SendEventTrackingData(EventTracking eventTracking)
        {
            var path = "events/v3/send";
            return _client.ExecuteAsync(path, eventTracking, Method.Post, convertToPropertiesSchema: false);
        }
        

        public Task<T> GetByNameAsync<T>(string eventName) where T : EventDefinition, new()
        {
            var path = $"{new T().RouteBasePath}/{eventName}";
            try
            {
                return _client.ExecuteAsync<T>(path, Method.Get, convertToPropertiesSchema: false);
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return Task.FromResult<T>(null);
                throw;
            }
        }
    }
}
