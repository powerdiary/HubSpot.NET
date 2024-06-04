using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HubSpot.NET.Api.CustomObject;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotCustomObjectApi
    {
        CustomObjectListHubSpotModel<T> List<T>(string idForCustomObject, ListRequestOptions opts = null)
            where T : CustomObjectHubSpotModel, new();

        Task<CustomObjectListHubSpotModel<T>> ListAsync<T>(string idForCustomObject, ListRequestOptions opts = null)
            where T : CustomObjectHubSpotModel, new();

        CustomObjectListAssociationsModel<T> GetAssociationsToCustomObject<T>(string objectTypeId,
            string customObjectId,
            string idForDesiredAssociation, CancellationToken cancellationToken)
            where T : CustomObjectAssociationModel, new();

        Task<CustomObjectListAssociationsModel<T>> GetAssociationsToCustomObjectAsync<T>(string objectTypeId,
            string customObjectId,
            string idForDesiredAssociation, CancellationToken cancellationToken)
            where T : CustomObjectAssociationModel, new();

        string CreateWithDefaultAssociationToObject<T>(T entity, string associateObjectType, string associateToObjectId)
            where T : CreateCustomObjectHubSpotModel, new();

        Task<string> CreateWithDefaultAssociationToObjectAsync<T>(T entity, string associateObjectType,
            string associateToObjectId)
            where T : CreateCustomObjectHubSpotModel, new();

        string UpdateObject<T>(T entity)
            where T : UpdateCustomObjectHubSpotModel, new();

        TReturn UpdateObject<TUpdate, TReturn>(TUpdate entity)
            where TUpdate : UpdateCustomObjectHubSpotModel, new()
            where TReturn : CustomObjectHubSpotModel, new();

        Task<TReturn> UpdateObjectAsync<TUpdate, TReturn>(TUpdate entity)
            where TUpdate : UpdateCustomObjectHubSpotModel, new()
            where TReturn : CustomObjectHubSpotModel, new();

        T GetEquipmentDataById<T>(string schemaId, string entityId, string properties = "")
            where T : HubspotEquipmentObjectModel, new();

        T GetObject<T>(string schemaId, string objectId, List<string> properties)
            where T : CustomObjectHubSpotModel, new();

        Task<T> GetObjectAsync<T>(string schemaId, string objectId, List<string> properties)
            where T : CustomObjectHubSpotModel, new();

        Task DeleteObjectAsync(string objectType, string objectId);

        void DeleteObject(string objectType, string objectId);
    }
}