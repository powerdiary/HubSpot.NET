using System.Threading.Tasks;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;

namespace HubSpot.NET.Core.Interfaces;

public interface IHubSpotCompanyApi
{
    T Create<T>(T entity) where T : CompanyHubSpotModel, new();
    Task<T> CreateAsync<T>(T entity) where T : CompanyHubSpotModel, new();

    void Delete(long companyId);
    Task DeleteAsync(long companyId);

    CompanySearchResultModel<T> GetByDomain<T>(string domain, CompanySearchByDomain options = null) where T : CompanyHubSpotModel, new();
    Task<CompanySearchResultModel<T>> GetByDomainAsync<T>(string domain, CompanySearchByDomain options = null) where T : CompanyHubSpotModel, new();

    CompanyListHubSpotModel<T> List<T>(ListRequestOptions opts = null) where T : CompanyHubSpotModel, new();
    Task<CompanyListHubSpotModel<T>> ListAsync<T>(ListRequestOptions opts = null) where T : CompanyHubSpotModel, new();

    T GetById<T>(long companyId) where T : CompanyHubSpotModel, new();
    Task<T> GetByIdAsync<T>(long companyId) where T : CompanyHubSpotModel, new();

    T Update<T>(T entity) where T : CompanyHubSpotModel, new();
    Task<T> UpdateAsync<T>(T entity) where T : CompanyHubSpotModel, new();

    CompanySearchHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
    Task<CompanySearchHubSpotModel<T>> SearchAsync<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();

    T GetAssociations<T>(T entity) where T : CompanyHubSpotModel, new();
    Task<T> GetAssociationsAsync<T>(T entity) where T : CompanyHubSpotModel, new();
}
