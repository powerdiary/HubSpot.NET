using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotClient
    {
        T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();

        T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new();

        T Execute<T>(string absoluteUriPath, Method method = Method.Get, bool convertToPropertiesSchema = true)
            where T : IHubSpotModel, new();

        T Execute<T>(string absoluteUriPath, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new();

        void Execute(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true);

        void Execute(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag);

        T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();

        T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new();

        T ExecuteMultipart<T>(string absoluteUriPath, byte[] data, string filename,
            Dictionary<string, string> parameters, Method method = Method.Post) where T : new();

        void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            bool convertToPropertiesSchema = true);

        void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag);

        Task<T> ExecuteAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();

        Task<T> ExecuteAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new();

        Task<T> ExecuteAsync<T>(string absoluteUriPath, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();

        Task<T> ExecuteAsync<T>(string absoluteUriPath, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new();

        Task ExecuteAsync(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true);

        Task ExecuteAsync(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag);

        Task<T> ExecuteListAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();

        Task<T> ExecuteListAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new();

        Task<T> ExecuteMultipartAsync<T>(string absoluteUriPath, byte[] data, string filename,
            Dictionary<string, string> parameters, Method method = Method.Post) where T : new();

        Task ExecuteBatchAsync(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            bool convertToPropertiesSchema = true);

        Task ExecuteBatchAsync(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag);
    }
}