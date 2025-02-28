using HubSpot.NET.Api.Associations.Dto;
using System.Threading.Tasks;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotAssociationsApi
    {
        void AssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId);

        Task AssociationToObjectAsync(string objectType, string objectId, string toObjectType, string toObjectId);

        void AssociationToObjectByLabel(string objectType, string objectId, string toObjectType, string toObjectId,
            string associationCategory, int associationTypeId);

        Task AssociationToObjectByLabelAsync(string objectType, string objectId, string toObjectType, string toObjectId,
            string associationCategory, int associationTypeId);

        Task<T> GetAssociationsAsync<T>(string objectType, string objectId, string toObjectType) where T : AssociationListHubSpotModel, new();
    }
}