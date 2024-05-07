using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.Company;

public sealed class HubSpotCompanyApiIntegrationTests : HubSpotIntegrationTestBase, IDisposable
{
    private readonly HubSpotCompanyApi _api;
    private readonly IList<long> _companiesToCleanup;

    public HubSpotCompanyApiIntegrationTests()
    {
        var client = new HubSpotBaseClient(ApiKey);
        _api = new HubSpotCompanyApi(client);
        _companiesToCleanup = new List<long>();
    }

    public void Dispose()
    {
        foreach (var companyId in _companiesToCleanup)
        {
            _api.Delete(companyId);
        }
    }

    private CompanyHubSpotModel CreateTestCompany()
    {
        var newCompany = new CompanyHubSpotModel
        {
            Name = "Test Company",
            Country = "Australia",
            Website = "https://www.test.com"
        };
        var createdCompany = _api.Create(newCompany);

        createdCompany.Should().NotBeNull();
        createdCompany.Id.Should().HaveValue();
        _companiesToCleanup.Add(createdCompany.Id.Value);

        return createdCompany;
    }

    [Fact]
    public void CreateAndDeleteCompany()
    {
        CreateTestCompany();
    }

    [Fact]
    public void GetCompanyById()
    {
        var createdCompany = CreateTestCompany();

        var companyById = _api.GetById<CompanyHubSpotModel>(createdCompany.Id.Value);
        using (new AssertionScope())
        {
            companyById.Should().NotBeNull();
            companyById.Id.Should().Be(createdCompany.Id);
            companyById.Name.Should().Be(createdCompany.Name);
            companyById.Country.Should().Be(createdCompany.Country);
            companyById.Website.Should().Be(createdCompany.Website);
        }
    }
}