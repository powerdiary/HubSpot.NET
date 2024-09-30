using System.Threading.Tasks;
using HubSpot.NET.Api.LineItem;
using HubSpot.NET.Api.LineItem.DTO;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotLineItemApi
    {
        Task<TResponse> CreateAsync<TRequest, TResponse>(TRequest entity)
            where TRequest : LineItemCreateOrUpdateRequest, new()
            where TResponse : LineItemGetResponse, new();

        Task DeleteAsync(long lineItemId);

        Task<T> GetByIdAsync<T>(long lineItemId, LineItemListRequestOptions requestOptions = null) where T : LineItemGetResponse, new();

        Task<TResponse> UpdateAsync<TRequest, TResponse>(TRequest entity)
            where TRequest : LineItemCreateOrUpdateRequest, new()
            where TResponse : LineItemGetResponse, new();

        Task<LineItemListHubSpotModel<T>> ListAsync<T>(LineItemListRequestOptions opts = null)
            where T : LineItemGetResponse, new();
    }
}