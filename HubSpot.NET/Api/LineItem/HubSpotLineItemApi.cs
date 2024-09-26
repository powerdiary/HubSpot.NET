using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HubSpot.NET.Api.LineItem.DTO;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.LineItem
{
    namespace HubSpot.NET.Api.LineItems
    {
        public class HubSpotLineItemApi : IHubSpotLineItemApi
        {
            private readonly IHubSpotClient _client;

            public HubSpotLineItemApi(IHubSpotClient client)
            {
                _client = client;
            }

            public Task<TResponse> CreateAsync<TRequest, TResponse>(TRequest entity)
                where TRequest : LineItemCreateOrUpdateRequest, new()
                where TResponse : LineItemGetResponse, new()
            {
                var path = "/crm/v3/objects/line_items";

                return _client.ExecuteAsync<TResponse>(path, entity, Method.Post, convertToPropertiesSchema: false);
            }

            public Task DeleteAsync(long lineItemId)
            {
                var path = $"/crm/v3/objects/line_items/{lineItemId}";
                return _client.ExecuteAsync(path, method: Method.Delete, convertToPropertiesSchema: false);
            }

            public async Task<T> GetByIdAsync<T>(long lineItemId, LineItemListRequestOptions requestOptions = null)
                where T : LineItemGetResponse, new()
            {
                requestOptions ??= new LineItemListRequestOptions();

                if (requestOptions.PropertiesToInclude == null || requestOptions.PropertiesToInclude.Count == 0)
                {
                    requestOptions.PropertiesToInclude = new List<string> { "name", "price" };
                }

                if (requestOptions.Associations == null || requestOptions.Associations.Count == 0)
                {
                    requestOptions.Associations = new List<string> { "deals" };
                }

                var propertiesQueryParam = string.Join(",", requestOptions.PropertiesToInclude);

                var associationsQueryParam =
                    requestOptions.Associations != null && requestOptions.Associations.Count > 0
                        ? $"&associations={string.Join(",", requestOptions.Associations)}"
                        : "";

                var path =
                    $"/crm/v3/objects/line_items/{lineItemId}?properties={propertiesQueryParam}{associationsQueryParam}";

                try
                {
                    return await _client.ExecuteAsync<T>(path, Method.Get, convertToPropertiesSchema: false);
                }
                catch (HubSpotException hubSpotEx)
                {
                    if (hubSpotEx.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new HubSpotException($"Line item with ID {lineItemId} does not exist.",
                            new HubSpotError(hubSpotEx.ReturnedError.StatusCode, hubSpotEx.ReturnedError.Description));
                    }

                    throw;
                }
            }

            public Task<TResponse> UpdateAsync<TRequest, TResponse>(TRequest entity)
                where TRequest : LineItemCreateOrUpdateRequest, new()
                where TResponse : LineItemGetResponse, new()
            {
                if (entity.Id < 1)
                    throw new ArgumentException("Line Item entity must have an id set!");

                var path = $"/crm/v3/objects/line_items/{entity.Id}";

                return _client.ExecuteAsync<TResponse>(path, entity, Method.Patch, convertToPropertiesSchema: false);
            }

            public Task<LineItemListHubSpotModel<T>> ListAsync<T>(LineItemListRequestOptions opts = null)
                where T : LineItemGetResponse, new()
            {
                if (opts == null)
                    opts = new LineItemListRequestOptions();

                var path = $"/crm/v3/objects/line_items"
                    .SetQueryParam("limit", opts.Limit);

                if (opts.Offset.HasValue)
                    path = path.SetQueryParam("after", opts.Offset);

                return _client.ExecuteListAsync<LineItemListHubSpotModel<T>>(path, convertToPropertiesSchema: false);
            }
        }
    }
}