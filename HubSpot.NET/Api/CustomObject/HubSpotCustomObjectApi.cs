using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.CustomObject
{
    public class HubSpotCustomObjectApi : IHubSpotCustomObjectApi
    {
        private readonly IHubSpotClient _client;

        private readonly string RouteBasePath = "crm/v3/objects";
        private readonly IHubSpotAssociationsApi _hubSpotAssociationsApi;

        public HubSpotCustomObjectApi(IHubSpotClient client, IHubSpotAssociationsApi hubSpotAssociationsApi)
        {
            _client = client;
            _hubSpotAssociationsApi = hubSpotAssociationsApi;
        }

        /// <summary>
        /// List all objects of a custom object type in your system
        /// </summary>
        /// <param name="idForCustomObject">Should be prefaced with "2-"</param>
        /// <param name="opts"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CustomObjectListHubSpotModel<T> List<T>(string idForCustomObject, ListRequestOptions opts = null)
            where T : CustomObjectHubSpotModel, new()
        {
            opts ??= new ListRequestOptions();

            var path = $"{RouteBasePath}/{idForCustomObject}"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("vidOffset", opts.Offset);

            var response = _client.ExecuteList<CustomObjectListHubSpotModel<T>>(path, convertToPropertiesSchema: false);
            return response;
        }

        /// <summary>
        /// Get the list of associations between two objects (BOTH CUSTOM and NOT)
        /// </summary>
        /// <param name="objectTypeId"></param>
        /// <param name="customObjectId"></param>
        /// <param name="idForDesiredAssociation"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CustomObjectListAssociationsModel<T> GetAssociationsToCustomObject<T>(string objectTypeId,
            string customObjectId,
            string idForDesiredAssociation, CancellationToken cancellationToken)
            where T : CustomObjectAssociationModel, new()
        {
            var path = $"{RouteBasePath}/{objectTypeId}/{customObjectId}/associations/{idForDesiredAssociation}";

            var response =
                _client.ExecuteList<CustomObjectListAssociationsModel<T>>(path, convertToPropertiesSchema: false);
            return response;
        }

        /// <summary>
        /// Adds the ability to create a custom object inside hubspot
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="associateObjectType"></param>
        /// <param name="associateToObjectId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string CreateWithDefaultAssociationToObject<T>(T entity, string associateObjectType,
            string associateToObjectId) where T : CreateCustomObjectHubSpotModel, new()
        {
            var path = $"{RouteBasePath}/{entity.SchemaId}";

            var response =
                _client.Execute<CreateCustomObjectHubSpotModel>(path, entity, Method.POST,
                    convertToPropertiesSchema: false);

            if (response.Properties.TryGetValue("hs_object_id", out var parsedId))
            {
                _hubSpotAssociationsApi.AssociationToObject(entity.SchemaId, parsedId.ToString(), associateObjectType,
                    associateToObjectId);
                return parsedId.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Update a custom object inside hubspot
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string UpdateObject<T>(T entity) where T : UpdateCustomObjectHubSpotModel, new()
        {
            var path = $"{RouteBasePath}/{entity.SchemaId}/{entity.Id}";

            _client.Execute<UpdateCustomObjectHubSpotModel>(path, entity, Method.PATCH,
                convertToPropertiesSchema: false);

            return string.Empty;
        }

        public T GetEquipmentDataById<T>(string schemaId, string entityId, string properties = "")
            where T : HubspotEquipmentObjectModel, new()
        {
            if (properties == "")
            {
                properties = EquipmentObjectList.GetEquipmentPropsList();
            }

            var path = $"{RouteBasePath}/{schemaId}/{entityId}";

            path = path.SetQueryParam("properties",
                properties); //properties is comma seperated value of properties to include

            var res = _client.Execute<T>(path, Method.GET, convertToPropertiesSchema: true);

            return res;
        }

        public async Task<CustomObjectListHubSpotModel<T>> ListAsync<T>(string idForCustomObject,
            ListRequestOptions opts = null) where T : CustomObjectHubSpotModel, new()
        {
            opts ??= new ListRequestOptions();

            var path = $"{RouteBasePath}/{idForCustomObject}"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("vidOffset", opts.Offset);

            var response =
                await _client.ExecuteListAsync<CustomObjectListHubSpotModel<T>>(path, convertToPropertiesSchema: false);
            return response;
        }

        public async Task<CustomObjectListAssociationsModel<T>> GetAssociationsToCustomObjectAsync<T>(
            string objectTypeId,
            string customObjectId,
            string idForDesiredAssociation, CancellationToken cancellationToken)
            where T : CustomObjectAssociationModel, new()
        {
            var path = $"{RouteBasePath}/{objectTypeId}/{customObjectId}/associations/{idForDesiredAssociation}";

            var response =
                await _client.ExecuteListAsync<CustomObjectListAssociationsModel<T>>(path,
                    convertToPropertiesSchema: false);
            return response;
        }

        public async Task<string> CreateWithDefaultAssociationToObjectAsync<T>(T entity, string associateObjectType,
            string associateToObjectId) where T : CreateCustomObjectHubSpotModel, new()
        {
            var path = $"{RouteBasePath}/{entity.SchemaId}";

            var response =
                await _client.ExecuteAsync<CreateCustomObjectHubSpotModel>(path, entity, Method.POST,
                    convertToPropertiesSchema: false);

            if (response.Properties.TryGetValue("hs_object_id", out var parsedId))
            {
                await _hubSpotAssociationsApi.AssociationToObjectAsync(entity.SchemaId, parsedId.ToString(),
                    associateObjectType, associateToObjectId);
                return parsedId.ToString();
            }

            return string.Empty;
        }

        public async Task<string> UpdateObjectAsync<T>(T entity) where T : UpdateCustomObjectHubSpotModel, new()
        {
            var path = $"{RouteBasePath}/{entity.SchemaId}/{entity.Id}";

            await _client.ExecuteAsync<UpdateCustomObjectHubSpotModel>(path, entity, Method.PATCH,
                convertToPropertiesSchema: false);

            return string.Empty;
        }

        public Task<T> GetObjectAsync<T>(string schemaId, string objectId)
            where T : CustomObjectHubSpotModel, new()
        {
            var path = $"{RouteBasePath}/{schemaId}/{objectId}";
            return _client.ExecuteAsync<T>(path, Method.GET, convertToPropertiesSchema: false);
        }

        public Task DeleteObjectAsync(string objectType, string objectId)
        {
            var path = $"{RouteBasePath}/{objectType}/{objectId}";
            return _client.ExecuteAsync(path, null, Method.DELETE, convertToPropertiesSchema: false);
        }
    }
}