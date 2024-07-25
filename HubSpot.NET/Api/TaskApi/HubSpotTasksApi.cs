using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HubSpot.NET.Api.TaskApi.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.TaskApi
{
    public class HubSpotTasksApi : IHubSpotTasksApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotTasksApi(IHubSpotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a Task entity
        /// </summary>
        /// <typeparam name="T">Implementation of TaskHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Create<T>(T entity) where T : HubSpotTaskModel, new()
        {
            string path = $"{entity.RouteBasePath}";

            return _client.Execute<T>(path, entity, Method.Post, SerialisationType.PropertyBag);
        }

        /// <summary>
        /// Gets a specific task by its ID
        /// </summary>
        /// <typeparam name="T">Implementation of TaskHubSpotModel</typeparam>
        /// <param name="dealId">The ID</param>
        /// <returns>The task entity or null if the task does not exist</returns>
        public T GetById<T>(long dealId, List<string> propertiesToInclude = null) where T : HubSpotTaskModel, new()
        {
            string path = $"{new T().RouteBasePath}/{dealId}";

            if (propertiesToInclude == null)
                propertiesToInclude = new List<string>
                {
                    "hs_task_subject", "hubspot_owner_id", "hs_task_body", "hs_task_status", "hs_task_priority",
                    "hs_task_type", "hs_timestamp"
                };

            if (propertiesToInclude.Any())
                path = path.SetQueryParam("properties", propertiesToInclude);

            try
            {
                return _client.Execute<T>(path, Method.Get, SerialisationType.PropertyBag);
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public HubSpotTaskListModel<T> List<T>(ListRequestOptions opts = null) where T : HubSpotTaskModel, new()
        {
            if (opts == null)
                opts = new ListRequestOptions();

            string path = $"{new T().RouteBasePath}"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("after", opts.Offset);

            HubSpotTaskListModel<T> data =
                _client.ExecuteList<HubSpotTaskListModel<T>>(path, convertToPropertiesSchema: true);

            return data;
        }

        /// <summary>
        /// Updates a given task entity, any changed properties are updated
        /// </summary>
        /// <typeparam name="T">Implementation of TaskHubSpotModel</typeparam>
        /// <param name="entity">The task entity</param>
        /// <returns>The updated task entity</returns>
        public T Update<T>(T entity) where T : HubSpotTaskModel, new()
        {
            if (entity.Id == null || entity.Id < 1)
                throw new ArgumentException("Task entity must have an id set!");

            long entityId = entity.Id.Value;
            string path = $"{entity.RouteBasePath}/{entity.Id}";

            T data = _client.Execute<T>(path, entity, Method.Patch, SerialisationType.PropertyBag);
            // this just undoes some dirty meddling
            entity.Id = entityId;

            return data;
        }

        /// <summary>
        /// Deletes the given task
        /// </summary>
        /// <param name="dealId">ID of the task</param>
        public void Delete(long dealId)
        {
            var path = $"{new HubSpotTaskModel().RouteBasePath}/{dealId}";

            _client.Execute(path, method: Method.Delete, convertToPropertiesSchema: true);
        }
    }
}