using System.Threading.Tasks;
using HubSpot.NET.Api.LineItem;
using HubSpot.NET.Api.LineItem.DTO;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotLineItemApi
    {
        Task<T> CreateAsync<T>(T entity) where T : LineItemHubSpotModel, new();

        Task DeleteAsync(long lineItemId);

        Task<T> GetByIdAsync<T>(long lineItemId, LineItemListRequestOptions requestOptions = null) where T : LineItemHubSpotModel, new();

        Task<T> UpdateAsync<T>(T entity) where T : LineItemHubSpotModel, new();

        Task<LineItemListHubSpotModel<T>> ListAsync<T>(LineItemListRequestOptions opts = null)
            where T : LineItemHubSpotModel, new();
    }
}