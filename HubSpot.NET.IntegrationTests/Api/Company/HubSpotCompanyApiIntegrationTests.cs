using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using SearchRequestFilter = HubSpot.NET.Api.SearchRequestFilter;
using SearchRequestFilterGroup = HubSpot.NET.Api.SearchRequestFilterGroup;

namespace HubSpot.NET.IntegrationTests.Api.Company;

public sealed class HubSpotCompanyApiIntegrationTests : HubSpotIntegrationTestBase
{
    [Fact]
    public void GetCompanyByDomain()
    {
        const string uniqueDomain = "https://www.unique-test-domain.com";

        var createdCompany = CreateTestCompany(website: uniqueDomain);
        var companyByDomain = CompanyApi.GetByDomain<CompanyHubSpotModel>(createdCompany.Domain,
            new CompanySearchByDomain
            {
                RequestOptions = new CompanySearchByDomainRequestOptions
                {
                    Properties = new List<string> { "Name", "Country", "Website", "Domain", "CreatedAt", "UpdatedAt" }
                }
            });

        using (new AssertionScope())
        {
            var matchingCompany = companyByDomain.Results.FirstOrDefault(x=> x.Id == createdCompany.Id);

            matchingCompany.Should().NotBeNull("Expected a company, but found none.");
            matchingCompany.Should().BeEquivalentTo(createdCompany,
                options => options
                    .Excluding(c => c.Id));
        }
    }

    [Fact]
    public void CreateCompany()
    {
        const string expectedName = "Test Company";
        const string expectedCountry = "Test Country";
        const string expectedWebsite = "https://www.test.com";

        var createdCompany = CreateTestCompany(expectedName, expectedCountry, expectedWebsite);

        using (new AssertionScope())
        {
            createdCompany.Should().NotBeNull();
            createdCompany.Name.Should().Be(expectedName);
            createdCompany.Country.Should().Be(expectedCountry);
            createdCompany.Website.Should().Be(expectedWebsite);
        }
    }

    [Fact]
    public void DeleteCompany()
    {
        var createdCompany = CreateTestCompany();
        CompanyApi.Delete(createdCompany.Id.Value);
        CompanyApi.GetById<CompanyHubSpotModel>(createdCompany.Id.Value).Should().BeNull();
    }

    [Fact]
    public void DeleteCompany_WhenCompanyDoesNotExist_ShouldNotThrowException()
    {
        var act = () => CompanyApi.Delete(0);
        act.Should().NotThrow<HubSpotException>();
    }

    [Fact]
    public void GetCompanyById()
    {
        var createdCompany = CreateTestCompany();

        var companyById = CompanyApi.GetById<CompanyHubSpotModel>(createdCompany.Id.Value);
        using (new AssertionScope())
        {
            companyById.Should().NotBeNull();
            companyById.Id.Should().Be(createdCompany.Id);
            companyById.Name.Should().Be(createdCompany.Name);
            companyById.Country.Should().Be(createdCompany.Country);
            companyById.Website.Should().Be(createdCompany.Website);
        }
    }

    [Fact(Skip = "The API should be upgraded to V3")]
    public async Task GetCompanyById_WhenAssociationIsPresent_ShouldFetchAssociations()
    {
        var expectedCompany = CreateTestCompany();
        var expectedContact = RecreateTestContact();

        var expectedObjectType = "Company";
        var expectedObjectId = expectedCompany.Id.Value.ToString();
        var expectedToObjectType = "Contact";
        var expectedToObjectId = expectedContact.Id.Value.ToString();

        AssociationsApi.AssociationToObject(expectedObjectType, expectedObjectId, expectedToObjectType,
            expectedToObjectId);

        // Need to wait for data to be searchable.
        await Task.Delay(7000);

        var fetchedCompany = CompanyApi.GetById<CompanyHubSpotModel>(expectedCompany.Id.Value);

        using (new AssertionScope())
        {
            fetchedCompany.Associations.Should().NotBeNull("Expected to retrieve associations set for the company.");
            fetchedCompany.Associations.AssociatedContacts.Should().Contain(expectedContact.Id.Value,
                "The associated contact ID should be present in the retrieved associations.");
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
        var allCompanies = CompanyApi.List<CompanyHubSpotModel>(requestOptions);

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

        var updatedCompany = CompanyApi.Update(createdCompany);

        var retrievedCompany = CompanyApi.GetById<CompanyHubSpotModel>(updatedCompany.Id.Value);

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

        var searchResults = CompanyApi.Search<CompanyHubSpotModel>(searchOptions);

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
    public async Task GetCompanyAssociations()
    {
        var createdCompany = CreateTestCompany("Test Company", "Test Country", "https://www.test1.com");
        var createdContact = RecreateTestContact("testmail@test1.com", "TestFirstName", "TestLastName");

        AssociateContactWithCompany(createdCompany, createdContact); // replace with your actual method

        // Delay is required to allow time for new data to be searchable.
        await Task.Delay(7000);

        var associations = CompanyApi.GetAssociations(createdCompany);

        using (new AssertionScope())
        {
            associations.Should().NotBeNull("Expected to retrieve the associations set for the company.");
            associations.Associations.AssociatedContacts.Should().Contain(createdContact.Id.Value,
                "The associated contact ID should be present in the retrieved associations.");
        }
    }
}