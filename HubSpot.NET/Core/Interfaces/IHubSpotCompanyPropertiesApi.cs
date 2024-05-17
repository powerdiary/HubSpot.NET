using System.Threading.Tasks;
using HubSpot.NET.Api.Properties.Dto;

namespace HubSpot.NET.Core.Interfaces
{

    public interface IHubSpotCompanyPropertiesApi
    {
        PropertiesListHubSpotModel<CompanyPropertyHubSpotModel> GetAll();
        Task<PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>> GetAllAsync();

        CompanyPropertyHubSpotModel Create(CompanyPropertyHubSpotModel property);
        Task<CompanyPropertyHubSpotModel> CreateAsync(CompanyPropertyHubSpotModel property);

        CompanyPropertyHubSpotModel Update(CompanyPropertyHubSpotModel property);
        Task<CompanyPropertyHubSpotModel> UpdateAsync(CompanyPropertyHubSpotModel property);

        void Delete(string propertyName);
        Task DeleteAsync(string propertyName);
    }
}