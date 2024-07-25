using System.Threading.Tasks;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.Associations
{
    public class HubSpotAssociationsApi : IHubSpotAssociationsApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotAssociationsApi(IHubSpotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Adds the ability to associate via the default association
        /// See the PUT documentation here: https://developers.hubspot.com/docs/api/crm/associations
        /// </summary>
        /// <param name="objectType">the type of the object you're associating (e.g. contact).</param>
        /// <param name="objectId"> the ID of the record to associate.</param>
        /// <param name="toObjectType"> the ID of the record to associate.</param>
        /// <param name="toObjectId"> the ID of the record to associate.</param>
        public void AssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId)
        {
            var associationPath =
                $"/crm/v4/objects/{objectType}/{objectId}/associations/default/{toObjectType}/{toObjectId}";
            _client.Execute(associationPath, null, Method.Put, convertToPropertiesSchema: false);

        }

        /// <summary>
        /// Adds the ability to associate via the label association association
        /// See the PUT documentation here: https://developers.hubspot.com/docs/api/crm/associations
        /// </summary>
        /// <param name="objectType">the type of the object you're associating (e.g. contact).</param>
        /// <param name="objectId"> the ID of the record to associate.</param>
        /// <param name="toObjectType"> the ID of the record to associate.</param>
        /// <param name="toObjectId"> the ID of the record to associate.</param>
        /// <param name="associationCategory">Category type: HUBSPOT_DEFINED, INTEGRATOR_DEFINED, USER_DEFINED</param>
        /// <param name="associationTypeId">This is the ID of the label, can be hard to find, the url of the label in your settings is a good place to look</param>
        public void AssociationToObjectByLabel(string objectType, string objectId, string toObjectType,
            string toObjectId, string associationCategory, int associationTypeId)
        {
            var associationPath =
                $"/crm/v4/objects/{objectType}/{objectId}/associations/{toObjectType}/{toObjectId}";
            var label = new
            {
                associationCategory,
                associationTypeId
            };
            var body = new[] { label };
            _client.Execute(associationPath, body, Method.Put, convertToPropertiesSchema: false);
        }

        public Task AssociationToObjectAsync(string objectType, string objectId, string toObjectType, string toObjectId)
        {
            var associationPath =
                $"/crm/v4/objects/{objectType}/{objectId}/associations/default/{toObjectType}/{toObjectId}";
            return _client.ExecuteAsync(associationPath, null, Method.Put, convertToPropertiesSchema: false);
        }

        public Task AssociationToObjectByLabelAsync(string objectType, string objectId, string toObjectType,
            string toObjectId,
            string associationCategory, int associationTypeId)
        {
            var associationPath =
                $"/crm/v4/objects/{objectType}/{objectId}/associations/{toObjectType}/{toObjectId}";
            var label = new
            {
                associationCategory,
                associationTypeId
            };
            var body = new[] { label };
            return _client.ExecuteAsync(associationPath, body, Method.Put, convertToPropertiesSchema: false);
        }
    }
}