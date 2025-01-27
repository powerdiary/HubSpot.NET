﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Contact.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotContactApi
    {
        T Create<T>(T entity) where T : ContactHubSpotModel, new();
        Task<T> CreateAsync<T>(T entity) where T : ContactHubSpotModel, new();

        T CreateOrUpdate<T>(T entity) where T : ContactHubSpotModel, new();
        Task<T> CreateOrUpdateAsync<T>(T entity) where T : ContactHubSpotModel, new();

        void Delete(long contactId);
        Task DeleteAsync(long contactId);

        void Batch<T>(List<T> entities) where T : ContactHubSpotModel, new();
        Task BatchAsync<T>(List<T> entities) where T : ContactHubSpotModel, new();

        T GetByEmail<T>(string email) where T : ContactHubSpotModel, new();
        Task<T> GetByEmailAsync<T>(string email) where T : ContactHubSpotModel, new();

        T GetById<T>(long contactId) where T : ContactHubSpotModel, new();
        Task<T> GetByIdAsync<T>(long contactId) where T : ContactHubSpotModel, new();

        T GetByUserToken<T>(string userToken) where T : ContactHubSpotModel, new();
        Task<T> GetByUserTokenAsync<T>(string userToken) where T : ContactHubSpotModel, new();

        ContactListHubSpotModel<T> List<T>(ListRequestOptions opts = null) where T : ContactHubSpotModel, new();

        Task<ContactListHubSpotModel<T>> ListAsync<T>(ListRequestOptions opts = null)
            where T : ContactHubSpotModel, new();

        void Update<T>(T contact) where T : ContactHubSpotModel, new();
        Task UpdateAsync<T>(T contact) where T : ContactHubSpotModel, new();

        ContactListHubSpotModel<T> RecentlyCreated<T>(ListRecentRequestOptions opts = null)
            where T : ContactHubSpotModel, new();

        Task<ContactListHubSpotModel<T>> RecentlyCreatedAsync<T>(ListRecentRequestOptions opts = null)
            where T : ContactHubSpotModel, new();

        ContactListHubSpotModel<T> RecentlyUpdated<T>(ListRecentRequestOptions opts = null)
            where T : ContactHubSpotModel, new();

        Task<ContactListHubSpotModel<T>> RecentlyUpdatedAsync<T>(ListRecentRequestOptions opts = null)
            where T : ContactHubSpotModel, new();

        ContactSearchHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();

        Task<ContactSearchHubSpotModel<T>> SearchAsync<T>(SearchRequestOptions opts = null)
            where T : ContactHubSpotModel, new();
    }
}