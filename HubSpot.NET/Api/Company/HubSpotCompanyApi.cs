using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.Company
{
    public class HubSpotCompanyApi : IHubSpotCompanyApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotCompanyApi(IHubSpotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a Company entity
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Create<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            var path = $"{entity.RouteBasePath}/companies";

            return _client.Execute<T>(path, entity, Method.Post, convertToPropertiesSchema: true);
        }

        /// <summary>
        /// Gets a specific company by it's ID
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="companyId">The ID</param>
        /// <returns>The company entity or null if the company does not exist</returns>
        public T GetById<T>(long companyId) where T : CompanyHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/companies/{companyId}";

            try
            {
                return _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Gets a company by domain name
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="domain">Domain name to search for</param>
        /// <param name="options">Set of search options</param>
        /// <returns>The company entity or null if the company does not exist</returns>
        public CompanySearchResultModel<T> GetByDomain<T>(string domain, CompanySearchByDomain options = null)
            where T : CompanyHubSpotModel, new()
        {
            if (options == null)
                options = new CompanySearchByDomain();

            var path = $"{new CompanyHubSpotModel().RouteBasePath}/domains/{domain}/companies";

            try
            {
                CompanySearchResultModel<T> data =
                    _client.ExecuteList<CompanySearchResultModel<T>>(path, options, Method.Post,
                        convertToPropertiesSchema: true);

                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public CompanyListHubSpotModel<T> List<T>(ListRequestOptions opts = null) where T : CompanyHubSpotModel, new()
        {
            if (opts == null)
                opts = new ListRequestOptions();

            var path = $"{new CompanyHubSpotModel().RouteBasePath}/companies/paged"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("offset", opts.Offset);

            CompanyListHubSpotModel<T> data =
                _client.ExecuteList<CompanyListHubSpotModel<T>>(path, convertToPropertiesSchema: true);

            return data;
        }

        /// <summary>
        /// Updates a given company entity, any changed properties are updated
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="entity">The company entity</param>
        /// <returns>The updated company entity</returns>
        public T Update<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            if (entity.Id < 1)
                throw new ArgumentException("Company entity must have an id set!");

            var path = $"{entity.RouteBasePath}/companies/{entity.Id}";

            T data = _client.Execute<T>(path, entity, Method.Put, convertToPropertiesSchema: true);

            return data;
        }

        /// <summary>
        /// Deletes the given company
        /// </summary>
        /// <param name="companyId">ID of the company</param>
        public void Delete(long companyId)
        {
            var path = $"{new CompanyHubSpotModel().RouteBasePath}/companies/{companyId}";

            _client.Execute(path, method: Method.Delete, convertToPropertiesSchema: true);
        }

        public CompanySearchHubSpotModel<T> Search<T>(SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new()
        {
            if (opts == null)
                opts = new SearchRequestOptions();

            var path = "/crm/v3/objects/companies/search";

            CompanySearchHubSpotModel<T> data =
                _client.ExecuteList<CompanySearchHubSpotModel<T>>(path, opts, Method.Post,
                    convertToPropertiesSchema: true);

            return data;
        }

        /// <summary>
        /// Gets a list of associations for a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="CompanyHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to get associations for</param>
        public T GetAssociations<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var companyPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/6";
            long? offSet = null;

            var dealResults = new List<long>();
            do
            {
                var dealAssociations = _client.ExecuteList<AssociationIdListHubSpotModel>(
                    string.Format("{0}?limit=100{1}", companyPath, offSet == null ? null : "&offset=" + offSet),
                    convertToPropertiesSchema: false);
                if (dealAssociations.Results.Any())
                    dealResults.AddRange(dealAssociations.Results);
                if (dealAssociations.HasMore)
                    offSet = dealAssociations.Offset;
                else
                    offSet = null;
            } while (offSet != null);

            if (dealResults.Any())
                entity.Associations.AssociatedDeals = dealResults.ToArray();
            else
                entity.Associations.AssociatedDeals = null;

            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var contactPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/2";

            var contactResults = new List<long>();
            do
            {
                var contactAssociations = _client.ExecuteList<AssociationIdListHubSpotModel>(
                    string.Format("{0}?limit=100{1}", contactPath, offSet == null ? null : "&offset=" + offSet),
                    convertToPropertiesSchema: false);
                if (contactAssociations.Results.Any())
                    contactResults.AddRange(contactAssociations.Results);
                if (contactAssociations.HasMore)
                    offSet = contactAssociations.Offset;
                else
                    offSet = null;
            } while (offSet != null);

            if (contactResults.Any())
                entity.Associations.AssociatedContacts = contactResults.ToArray();
            else
                entity.Associations.AssociatedContacts = null;

            return entity;
        }

        public Task<T> CreateAsync<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            var path = $"{entity.RouteBasePath}/companies";

            return _client.ExecuteAsync<T>(path, entity, Method.Post, convertToPropertiesSchema: true);
        }

        public Task DeleteAsync(long companyId)
        {
            var path = $"{new CompanyHubSpotModel().RouteBasePath}/companies/{companyId}";

            return _client.ExecuteAsync(path, method: Method.Delete, convertToPropertiesSchema: true);
        }

        public async Task<CompanySearchResultModel<T>> GetByDomainAsync<T>(string domain,
            CompanySearchByDomain options = null) where T : CompanyHubSpotModel, new()
        {
            if (options == null)
                options = new CompanySearchByDomain();

            var path = $"{new CompanyHubSpotModel().RouteBasePath}/domains/{domain}/companies";

            try
            {
                CompanySearchResultModel<T> data =
                    await _client.ExecuteListAsync<CompanySearchResultModel<T>>(path, options, Method.Post,
                        convertToPropertiesSchema: true);

                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public Task<CompanyListHubSpotModel<T>> ListAsync<T>(ListRequestOptions opts = null)
            where T : CompanyHubSpotModel, new()
        {
            if (opts == null)
                opts = new ListRequestOptions();

            var path = $"{new CompanyHubSpotModel().RouteBasePath}/companies/paged"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("offset", opts.Offset);

            return _client.ExecuteListAsync<CompanyListHubSpotModel<T>>(path, convertToPropertiesSchema: true);
        }

        public async Task<T> GetByIdAsync<T>(long companyId) where T : CompanyHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/companies/{companyId}";

            try
            {
                return await _client.ExecuteAsync<T>(path, Method.Get, convertToPropertiesSchema: true);
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public async Task<T> UpdateAsync<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            if (entity.Id < 1)
                throw new ArgumentException("Company entity must have an id set!");

            var path = $"{entity.RouteBasePath}/companies/{entity.Id}";

            T data = await _client.ExecuteAsync<T>(path, entity, Method.Put, convertToPropertiesSchema: true);

            return data;
        }

        public Task<CompanySearchHubSpotModel<T>> SearchAsync<T>(SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new()
        {
            if (opts == null)
                opts = new SearchRequestOptions();

            var path = "/crm/v3/objects/companies/search";

            return _client.ExecuteListAsync<CompanySearchHubSpotModel<T>>(path, opts, Method.Post,
                convertToPropertiesSchema: true);
        }

        public async Task<T> GetAssociationsAsync<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            var dealPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/6";
            var associatedDeals = await GetAssociationsForPath(dealPath);
            entity.Associations.AssociatedDeals = associatedDeals.Any() ? associatedDeals.ToArray() : null;

            var contactPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/2";
            var associatedContacts = await GetAssociationsForPath(contactPath);
            entity.Associations.AssociatedContacts = associatedContacts.Any() ? associatedContacts.ToArray() : null;

            return entity;
        }

        private async Task<List<long>> GetAssociationsForPath(string path)
        {
            var results = new List<long>();
            long? offset = null;

            do
            {
                var associations = await _client.ExecuteListAsync<AssociationIdListHubSpotModel>(
                    $"{path}?limit=100{(offset == null ? "" : $"&offset={offset}")}",
                    convertToPropertiesSchema: false);

                if (associations.Results.Any())
                    results.AddRange(associations.Results);

                offset = associations.HasMore ? associations.Offset : null;
            } while (offset != null);

            return results;
        }
    }
}