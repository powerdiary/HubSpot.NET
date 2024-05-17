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

        Task<string> UpdateObjectAsync<T>(T entity)
            where T : UpdateCustomObjectHubSpotModel, new();

        T GetEquipmentDataById<T>(string schemaId, string entityId, string properties = "")
            where T : HubspotEquipmentObjectModel, new();

        Task<T> GetEquipmentDataByIdAsync<T>(string schemaId, string entityId, string properties = "")
            where T : HubspotEquipmentObjectModel, new();
    }
}