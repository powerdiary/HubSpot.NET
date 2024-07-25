using System.Threading.Tasks;
using HubSpot.NET.Api.Properties.Dto;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.Properties
{
    public class HubSpotCompaniesPropertiesApi : IHubSpotCompanyPropertiesApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotCompaniesPropertiesApi(IHubSpotClient client)
        {
            _client = client;
        }

        public PropertiesListHubSpotModel<CompanyPropertyHubSpotModel> GetAll()
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}";

            return _client.ExecuteList<PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>>(path,
                convertToPropertiesSchema: false);
        }

        public CompanyPropertyHubSpotModel Create(CompanyPropertyHubSpotModel property)
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}";

            return _client.Execute<CompanyPropertyHubSpotModel>(path, property, Method.Post,
                convertToPropertiesSchema: false);
        }

        public CompanyPropertyHubSpotModel Update(CompanyPropertyHubSpotModel property)
        {
            var path =
                $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}/named/{property.Name}";

            return _client.Execute<CompanyPropertyHubSpotModel>(path, property, Method.Put,
                convertToPropertiesSchema: false);
        }

        public void Delete(string propertyName)
        {
            var path =
                $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}/named/{propertyName}";

            _client.Execute(path, method: Method.Delete, convertToPropertiesSchema: true);
        }

        public Task<PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>> GetAllAsync()
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}";

            return _client.ExecuteListAsync<PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>>(path,
                convertToPropertiesSchema: false);
        }

        public Task<CompanyPropertyHubSpotModel> CreateAsync(CompanyPropertyHubSpotModel property)
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}";

            return _client.ExecuteAsync<CompanyPropertyHubSpotModel>(path, property, Method.Post,
                convertToPropertiesSchema: false);
        }

        public Task<CompanyPropertyHubSpotModel> UpdateAsync(CompanyPropertyHubSpotModel property)
        {
            var path =
                $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}/named/{property.Name}";

            return _client.ExecuteAsync<CompanyPropertyHubSpotModel>(path, property, Method.Put,
                convertToPropertiesSchema: false);
        }

        public Task DeleteAsync(string propertyName)
        {
            var path =
                $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}/named/{propertyName}";

            return _client.ExecuteAsync(path, method: Method.Delete, convertToPropertiesSchema: true);
        }
    }
}