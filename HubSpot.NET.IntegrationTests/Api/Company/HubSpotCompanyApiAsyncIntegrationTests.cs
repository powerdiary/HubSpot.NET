using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using SearchRequestFilter = HubSpot.NET.Api.SearchRequestFilter;
using SearchRequestFilterGroup = HubSpot.NET.Api.SearchRequestFilterGroup;

namespace HubSpot.NET.IntegrationTests.Api.Company;

public sealed class HubSpotCompanyApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact]
    public async Task GetCompanyByDomainAsync()
    {
        const string uniqueDomain = "https://www.unique-test-domain.com";

        var createdCompany = await RecreateTestCompanyAsync(website: uniqueDomain); // Use await and async method
        var companyByDomain = await CompanyApi.GetByDomainAsync<CompanyHubSpotModel>(
            createdCompany.Domain,
            new CompanySearchByDomain
            {
                RequestOptions = new CompanySearchByDomainRequestOptions
                {
                    Properties = new List<string> { "Name", "Country", "Website", "Domain", "CreatedAt", "UpdatedAt" }
                }
            });

        using (new AssertionScope())
        {
            var matchingCompany = companyByDomain.Results.FirstOrDefault(x => x.Id == createdCompany.Id);

            matchingCompany.Should().NotBeNull("Expected a company, but found none.");
            matchingCompany.Should().BeEquivalentTo(createdCompany,
                options => options.Excluding(c => c.Id));
        }
    }

    [Fact]
    public async Task CreateCompanyAsync()
    {
        const string expectedName = "Test Company";
        const string expectedCountry = "Test Country";
        const string expectedWebsite = "https://www.test.com";

        var createdCompany =
            await RecreateTestCompanyAsync(expectedName, expectedCountry,
                expectedWebsite);

        using (new AssertionScope())
        {
            createdCompany.Should().NotBeNull();
            createdCompany.Name.Should().Be(expectedName);
            createdCompany.Country.Should().Be(expectedCountry);
            createdCompany.Website.Should().Be(expectedWebsite);
        }
    }

    [Fact]
    public async Task DeleteCompanyAsync()
    {
        var createdCompany = await RecreateTestCompanyAsync();
        await CompanyApi.DeleteAsync(createdCompany.Id.Value);
        var company =
            await CompanyApi.GetByIdAsync<CompanyHubSpotModel>(createdCompany.Id.Value);
        company.Should().BeNull();
    }

    [Fact]
    public async Task
        DeleteCompany_WhenCompanyDoesNotExist_ShouldNotThrowExceptionAsync()
    {
        var act = new Func<Task>(() => CompanyApi.DeleteAsync(0));
        await act.Should().NotThrowAsync<HubSpotException>();
    }

    [Fact]
    public async Task GetCompanyByIdAsync()
    {
        var createdCompany = await RecreateTestCompanyAsync();

        var companyById =
            await CompanyApi.GetByIdAsync<CompanyHubSpotModel>(createdCompany.Id.Value);
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
    public async Task ListCompaniesAsync()
    {
        var uniqueId1 = Guid.NewGuid().ToString("N");
        var uniqueId2 = Guid.NewGuid().ToString("N");

        var firstCreatedCompany = await RecreateTestCompanyAsync("First Company " + uniqueId1);
        var secondCreatedCompany = await RecreateTestCompanyAsync("Second Company " + uniqueId2);

        await Task.Delay(10000);

        var companyList = await CompanyApi.ListAsync<CompanyHubSpotModel>(new ListRequestOptions
        {
            PropertiesToInclude = new List<string> { "Name" }
        });

        using (new AssertionScope())
        {
            companyList.Should().NotBeNull();

            if (companyList.Companies.Count < 20)
            {
                var companies = companyList.Companies.Where(company =>
                    company.Name == firstCreatedCompany.Name ||
                    company.Name == secondCreatedCompany.Name);

                companies.Should().HaveCount(2);
            }
        }
    }

    [Fact]
    public async Task UpdateCompanyAsync()
    {
        var createdCompany = await RecreateTestCompanyAsync("Test Company", "Country", "https://www.test.com");

        createdCompany.Name = "Updated Test Company";
        createdCompany.Country = "Updated Country";
        createdCompany.Website = "https://www.updatedtest.com";

        var updatedCompany = await CompanyApi.UpdateAsync(createdCompany);

        var retrievedCompany = await CompanyApi.GetByIdAsync<CompanyHubSpotModel>(updatedCompany.Id.Value);

        using (new AssertionScope())
        {
            retrievedCompany.Should().NotBeNull();
            retrievedCompany.Should().BeEquivalentTo(updatedCompany, options => options.Excluding(c => c.Id));
        }
    }

    [Fact]
    public async Task SearchCompaniesAsync()
    {
        var createdCompany = await RecreateTestCompanyAsync(name: "Search Test Company");

        // Delay is required to allow time for new data to be searchable.
        await Task.Delay(10000);

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

        var searchResults = await CompanyApi.SearchAsync<CompanyHubSpotModel>(searchOptions);

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

    [Fact]
    public async Task GetCompanyAssociationsAsync()
    {
        var createdCompany = await RecreateTestCompanyAsync("Test Company", "Test Country", "https://www.test1.com");
        var createdContact = await RecreateTestContactAsync("testmail@test1.com", "TestFirstName", "TestLastName");

        await AssociateContactWithCompanyAsync(createdCompany, createdContact);

        // Delay is required to allow time for new data to be searchable.
        await Task.Delay(7000);

        var associations = await CompanyApi.GetAssociationsAsync(createdCompany);

        using (new AssertionScope())
        {
            associations.Should().NotBeNull("Expected to retrieve the associations set for the company.");
            associations.Associations.AssociatedContacts.Should().Contain(createdContact.Id.Value,
                "The associated contact ID should be present in the retrieved associations.");
        }
    }
}