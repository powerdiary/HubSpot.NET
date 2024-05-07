using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.Company;

public class HubSpotCompanyApiIntegrationTests: HubSpotIntegrationTestBase
{
    private readonly HubSpotCompanyApi _api;

    public HubSpotCompanyApiIntegrationTests()
    {
        var client = new HubSpotBaseClient(ApiKey);
        _api = new HubSpotCompanyApi(client);
    }

    [Fact]
    public void CreateAndDeleteCompany()
    {
        CompanyHubSpotModel? createdCompany = null;

        try
        {
            var newCompany = new CompanyHubSpotModel
            {
                Name = "Test Company",
                Country = "Australia",
                Website = "https://www.test.com"
            };

            createdCompany = _api.Create(newCompany);

            using (new AssertionScope())
            {
                createdCompany.Should().NotBeNull();
                createdCompany.Id.Should().HaveValue();
            }
        }
        finally
        {
            if (createdCompany != null)
            {
                _api.Delete(createdCompany.Id.Value);
            }
        }
    }
}