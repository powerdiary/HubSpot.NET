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

    [Fact]
    public void GetCompanyByDomain()
    {
        const string uniqueDomain = "https://www.unique-test-domain.com";

        var createdCompany = CreateTestCompany(website: uniqueDomain);
        var companyByDomain = _api.GetByDomain<CompanyHubSpotModel>(createdCompany.Domain);

        using (new AssertionScope())
        {
            var matchingCompany = companyByDomain.Results.FirstOrDefault();

            matchingCompany.Should().NotBeNull("Expected a company, but found none.");
            matchingCompany.Should().BeEquivalentTo(createdCompany,
                options => options
                    .Excluding(c => c.Id));
        }
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

    [Fact]
    public void ListCompanies()
    {
        var createdCompanies = new List<CompanyHubSpotModel>();

        for (int i = 0; i < 3; i++)
        {
            var createdCompany = CreateTestCompany($"Test Company {i}", $"Country {i}", $"https://www.test{i}.com");
            createdCompanies.Add(createdCompany);
        }

        var requestOptions = new ListRequestOptions
        {
            PropertiesToInclude = new List<string> { "Name", "Country", "Website", "Domain" },
            Limit = 100
        };
        var allCompanies = _api.List<CompanyHubSpotModel>(requestOptions);

        using (new AssertionScope())
        {
            foreach (var createdCompany in createdCompanies)
            {
                var existingCompany = allCompanies.Companies.FirstOrDefault(c => c.Id == createdCompany.Id);

                existingCompany.Should().NotBeNull($"Expected to find company with id {createdCompany.Id} in list.");
                existingCompany.Should().BeEquivalentTo(createdCompany, options => options.Excluding(c => c.Id),
                    "The existing company should be equivalent to the created company except for the Id.");
            }
        }
    }

    public void Dispose()
    {
        foreach (var companyId in _companiesToCleanup)
        {
            _api.Delete(companyId);
        }
    }

    private CompanyHubSpotModel CreateTestCompany(string? name = null, string? country = null, string? website = null)
    {
        var newCompany = new CompanyHubSpotModel
        {
            Name = name,
            Country = country,
            Website = website
        };
        var createdCompany = _api.Create(newCompany);

        createdCompany.Should().NotBeNull();
        createdCompany.Id.Should().HaveValue();
        _companiesToCleanup.Add(createdCompany.Id.Value);

        return createdCompany;
    }
}