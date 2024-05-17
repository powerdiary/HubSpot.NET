using System.Collections.Generic;
using HubSpot.NET.Api.TaskApi.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotTasksApi
    {
        T Create<T>(T entity) where T : HubSpotTaskModel, new();
        void Delete(long dealId);
        T GetById<T>(long dealId, List<string> propertiesToInclude = null) where T : HubSpotTaskModel, new();
        T Update<T>(T entity) where T : HubSpotTaskModel, new();

        HubSpotTaskListModel<T> List<T>(ListRequestOptions opts = null)
            where T : HubSpotTaskModel, new();
    }
}