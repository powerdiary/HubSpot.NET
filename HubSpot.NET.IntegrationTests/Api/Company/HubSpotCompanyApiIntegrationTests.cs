using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using SearchRequestFilter = HubSpot.NET.Api.SearchRequestFilter;
using SearchRequestFilterGroup = HubSpot.NET.Api.SearchRequestFilterGroup;

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

    [Fact]
    public void UpdateCompany()
    {
        var createdCompany = CreateTestCompany("Test Company", "Country", "https://www.test.com");

        createdCompany.Name = "Updated Test Company";
        createdCompany.Country = "Updated Country";
        createdCompany.Website = "https://www.updatedtest.com";

        var updatedCompany = _api.Update(createdCompany);

        var retrievedCompany = _api.GetById<CompanyHubSpotModel>(updatedCompany.Id.Value);

        using (new AssertionScope())
        {
            retrievedCompany.Should().NotBeNull();
            retrievedCompany.Should().BeEquivalentTo(updatedCompany, options => options.Excluding(c => c.Id));
        }
    }

    [Fact]
    public async Task SearchCompanies()
    {
        var createdCompany = CreateTestCompany(name: "Search Test Company");

        // This delay is necessary because after the creation of a new company, it can take
        // some time for this data to be indexed and thus become searchable.
        await Task.Delay(7000);

        var filterGroup = new SearchRequestFilterGroup { Filters = new List<SearchRequestFilter>() };
        filterGroup.Filters.Add(new SearchRequestFilter
        {
            PropertyName = "name",
            Operator = SearchRequestFilterOperatorType.EqualTo,
            Value = createdCompany.Name
        });

        var searchOptions = new SearchRequestOptions
        {
            FilterGroups = new List<SearchRequestFilterGroup>(),
            PropertiesToInclude = new List<string> { "Name", "Country", "Website", "Domain", "CreatedAt", "UpdatedAt" }
        };

        searchOptions.FilterGroups.Add(filterGroup);

        var searchResults = _api.Search<CompanyHubSpotModel>(searchOptions);

        using (new AssertionScope())
        {
            var foundCompany = searchResults.Results.FirstOrDefault(c => c.Id == createdCompany.Id);

            foundCompany.Should().NotBeNull("Expected to find the created company in search results.");
            foundCompany.CreatedAt.Should().NotBeNull("Expected CreatedAt to have a value.");
            foundCompany.UpdatedAt.Should().NotBeNull("Expected UpdatedAt to have a value.");
            foundCompany.Should().BeEquivalentTo(createdCompany, options => options.Excluding(c => c.Id)
                .Excluding(c => c.CreatedAt)
                .Excluding(c => c.UpdatedAt));

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