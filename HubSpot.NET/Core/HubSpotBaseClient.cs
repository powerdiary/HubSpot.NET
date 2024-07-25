using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.OAuth.Dto;
using HubSpot.NET.Core.Requests;
using RestSharp;

namespace HubSpot.NET.Core
{
    public sealed class HubSpotBaseClient : IHubSpotClient
    {
        private readonly RequestSerializer _serializer = new RequestSerializer(new RequestDataConverter());
        private RestClient _client;

        private static string BaseUrl => "https://api.hubapi.com";

        private readonly HubSpotAuthenticationMode _mode;

        // Used for HAPIKEY method

        private readonly string _apiKey;

        // Used for OAUTH
        private readonly HubSpotToken _token;

        private void Initialise()
        {
            _client = new RestClient(BaseUrl);
        }

        /// <summary>
        /// Creates a HubSpot client with the authentication scheme HAPIKEY.
        /// </summary>
        public HubSpotBaseClient(string privateAppKey)
        {
            _apiKey = privateAppKey;
            _mode = HubSpotAuthenticationMode.PRIVATE_APP_KEY;
            Initialise();
        }

        /// <summary>
        /// Creates a HubSpot client with the authentication scheme OAUTH.
        /// </summary>
        public HubSpotBaseClient(HubSpotToken token)
        {
            _token = token;
            _mode = HubSpotAuthenticationMode.OAUTH;
            Initialise();
        }

        public T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            return Execute<T>(absoluteUriPath, entity, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            string json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, serialisationType);

            T data = SendRequest(absoluteUriPath, method, json,
                responseData =>
                    (T)_serializer.DeserializeEntity<T>(responseData, serialisationType != SerialisationType.Raw));

            return data;
        }

        public T Execute<T>(string absoluteUriPath, Method method = Method.Get, bool convertToPropertiesSchema = true)
            where T : IHubSpotModel, new()
        {
            return Execute<T>(absoluteUriPath, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public T Execute<T>(string absoluteUriPath, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            T data = SendRequest(absoluteUriPath, method, null,
                responseData =>
                    (T)_serializer.DeserializeEntity<T>(responseData, serialisationType != SerialisationType.Raw));

            return data;
        }

        public void Execute(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true)
        {
            Execute(absoluteUriPath, entity, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public void Execute(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag)
        {
            string json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, serialisationType);

            SendRequest(absoluteUriPath, method, json);
        }

        public void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            bool convertToPropertiesSchema = true)
        {
            ExecuteBatch(absoluteUriPath, entities, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag)
        {
            string json = (method == Method.Get || entities == null)
                ? null
                : _serializer.SerializeEntity(entities, serialisationType);

            SendRequest(absoluteUriPath, method, json);
        }

        public T ExecuteMultipart<T>(string absoluteUriPath, byte[] data, string filename,
            Dictionary<string, string> parameters, Method method = Method.Post) where T : new()
        {
            string path = $"{BaseUrl}{absoluteUriPath}";
            var request = ConfigureRequestAuthentication(path, method, null);

            request.AddFile("file", data, filename);

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                if (string.IsNullOrEmpty(kvp.Value))
                {
                    continue;
                }

                request.AddParameter(kvp.Key, kvp.Value);
            }

            var response = _client.Execute<T>(request);

            T responseData = response.Data;

            if (!response.IsSuccessful())
                throw new HubSpotException("Error from HubSpot",
                    new HubSpotError(response.StatusCode, response.StatusDescription));

            return responseData;
        }

        public T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            return ExecuteList<T>(absoluteUriPath, entity, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            string json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, true);

            var data = SendRequest(
                absoluteUriPath,
                method,
                json,
                responseData =>
                    (T)_serializer.DeserializeListEntity<T>(responseData, serialisationType != SerialisationType.Raw));
            return data;
        }

        private T SendRequest<T>(string path, Method method, string json, Func<string, T> deserializeFunc)
            where T : IHubSpotModel, new()
        {
            string responseData = SendRequest(path, method, json);

            if (string.IsNullOrWhiteSpace(responseData))
                return default;

            return deserializeFunc(responseData);
        }

        private string SendRequest(string path, Method method, string json)
        {
            RestRequest request = ConfigureRequestAuthentication(path, method);

            if (method != Method.Get && !string.IsNullOrWhiteSpace(json))
                request.AddParameter("application/json", json, ParameterType.RequestBody);

            RestResponse response = _client.Execute(request);

            string responseData = response.Content;

            if (!response.IsSuccessful())
                throw new HubSpotException("Error from HubSpot",
                    new HubSpotError(response.StatusCode, response.StatusDescription), responseData);

            return responseData;
        }

        /// <summary>
        /// Configures a <see cref="RestRequest"/> based on the authentication scheme detected and configures the endpoint path relative to the base path.
        /// </summary>
        private RestRequest ConfigureRequestAuthentication(string path, Method method,
            string contentType = "application/json")
        {
#if NET451
            RestRequest request = new RestRequest(path, method);
            request.RequestFormat = DataFormat.Json;
#else
            RestRequest request = new RestRequest(path, method);
#endif
            switch (_mode)
            {
                case HubSpotAuthenticationMode.OAUTH:
                    request.AddHeader("Authorization", GetAuthHeader(_token));
                    break;
                case HubSpotAuthenticationMode.HAPIKEY:
                    throw new NotSupportedException(
                        "This authentication mode is no longer supported by hubspot as of November 30, 2020");
                default:
                    request.AddHeader("Authorization", $"Bearer {_apiKey}");

                    if (!contentType.IsNullOrEmpty())
                    {
                        request.AddHeader("Content-Type", contentType);
                    }

                    break;
            }

            return request;
        }

        private string GetAuthHeader(HubSpotToken token) => $"Bearer {token.AccessToken}";

        public Task<T> ExecuteAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            var serialisationType =
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw;

            var json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, serialisationType);

            return SendRequestAsync(absoluteUriPath, method, json, responseData =>
                (T)_serializer.DeserializeEntity<T>(responseData, serialisationType != SerialisationType.Raw));
        }

        public Task<T> ExecuteAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            var json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, serialisationType);

            return SendRequestAsync(absoluteUriPath, method, json, responseData =>
                (T)_serializer.DeserializeEntity<T>(responseData, serialisationType != SerialisationType.Raw));
        }

        public Task<T> ExecuteAsync<T>(string absoluteUriPath, Method method = Method.Get,
            bool convertToPropertiesSchema = true)
            where T : IHubSpotModel, new()
        {
            return ExecuteAsync<T>(absoluteUriPath, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public async Task<T> ExecuteAsync<T>(string absoluteUriPath, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            T data = await SendRequestAsync(absoluteUriPath, method, null,
                responseData =>
                    (T)_serializer.DeserializeEntity<T>(responseData, serialisationType != SerialisationType.Raw));

            return data;
        }

        public Task ExecuteAsync(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true)
        {
            return ExecuteAsync(absoluteUriPath, entity, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public async Task ExecuteAsync(string absoluteUriPath, object entity = null, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag)
        {
            string json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, serialisationType);

            await SendRequestAsync(absoluteUriPath, method, json);
        }

        public Task ExecuteBatchAsync(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            bool convertToPropertiesSchema = true)
        {
            return ExecuteBatchAsync(absoluteUriPath, entities, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public async Task ExecuteBatchAsync(string absoluteUriPath, List<object> entities, Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag)
        {
            string json = (method == Method.Get || entities == null)
                ? null
                : _serializer.SerializeEntity(entities, serialisationType);

            await SendRequestAsync(absoluteUriPath, method, json);
        }

        public async Task<T> ExecuteMultipartAsync<T>(string absoluteUriPath, byte[] data, string filename,
            Dictionary<string, string> parameters, Method method = Method.Post) where T : new()
        {
            string path = $"{BaseUrl}{absoluteUriPath}";
            RestRequest request = ConfigureRequestAuthentication(path, method, null);

            request.AddFile("file", data, filename);

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                if (string.IsNullOrEmpty(kvp.Value))
                {
                    continue;
                }

                request.AddParameter(kvp.Key, kvp.Value);
            }

            RestResponse<T> response = await _client.ExecuteAsync<T>(request);

            T responseData = response.Data;

            if (!response.IsSuccessful)
                throw new HubSpotException("Error from HubSpot",
                    new HubSpotError(response.StatusCode, response.StatusDescription));

            return responseData;
        }

        public Task<T> ExecuteListAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.Get,
            bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            return ExecuteListAsync<T>(absoluteUriPath, entity, method,
                convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }

        public async Task<T> ExecuteListAsync<T>(string absoluteUriPath, object entity = null,
            Method method = Method.Get,
            SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            string json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, true);

            var data = await SendRequestAsync(
                absoluteUriPath,
                method,
                json,
                responseData =>
                    (T)_serializer.DeserializeListEntity<T>(responseData, serialisationType != SerialisationType.Raw));
            return data;
        }

        private async Task<T> SendRequestAsync<T>(string path, Method method, string json,
            Func<string, T> deserializeFunc)
            where T : IHubSpotModel, new()
        {
            string responseData = await SendRequestAsync(path, method, json);

            if (string.IsNullOrWhiteSpace(responseData))
                return default;

            return deserializeFunc(responseData);
        }

        private async Task<string> SendRequestAsync(string path, Method method, string json)
        {
            RestRequest request = ConfigureRequestAuthentication(path, method);

            if (method != Method.Get && !string.IsNullOrWhiteSpace(json))
                request.AddParameter("application/json", json, ParameterType.RequestBody);

            RestResponse response = await _client.ExecuteAsync(request);

            string responseData = response.Content;

            if (!response.IsSuccessful)
                throw new HubSpotException("Error from HubSpot",
                    new HubSpotError(response.StatusCode, response.StatusDescription), responseData);

            return responseData;
        }
    }
}