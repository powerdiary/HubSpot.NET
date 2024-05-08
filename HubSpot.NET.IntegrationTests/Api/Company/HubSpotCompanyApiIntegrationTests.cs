using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using SearchRequestFilter = HubSpot.NET.Api.SearchRequestFilter;
using SearchRequestFilterGroup = HubSpot.NET.Api.SearchRequestFilterGroup;

namespace HubSpot.NET.IntegrationTests.Api.Company;

public sealed class HubSpotCompanyApiIntegrationTests : HubSpotIntegrationTestBase, IDisposable
{
    private readonly HubSpotCompanyApi _companyApi;
    private readonly HubSpotContactApi _contactApi;
    private readonly HubSpotApi _hubSpotApi;
    private readonly IList<long> _companiesToCleanup;
    private readonly IList<long> _contactsToCleanup;

    public HubSpotCompanyApiIntegrationTests()
    {
        var client = new HubSpotBaseClient(ApiKey);
        _companyApi = new HubSpotCompanyApi(client);
        _contactApi = new HubSpotContactApi(client);
        _hubSpotApi = new HubSpotApi(ApiKey);

        _companiesToCleanup = new List<long>();
        _contactsToCleanup = new List<long>();
    }

    [Fact]
    public void GetCompanyByDomain()
    {
        const string uniqueDomain = "https://www.unique-test-domain.com";

        var createdCompany = CreateTestCompany(website: uniqueDomain);
        var companyByDomain = _companyApi.GetByDomain<CompanyHubSpotModel>(createdCompany.Domain);

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

        var companyById = _companyApi.GetById<CompanyHubSpotModel>(createdCompany.Id.Value);
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
        var allCompanies = _companyApi.List<CompanyHubSpotModel>(requestOptions);

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

        var updatedCompany = _companyApi.Update(createdCompany);

        var retrievedCompany = _companyApi.GetById<CompanyHubSpotModel>(updatedCompany.Id.Value);

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

        var searchResults = _companyApi.Search<CompanyHubSpotModel>(searchOptions);

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
        var createdContact = CreateTestContact("testmail@test1.com", "TestFirstName", "TestLastName");

        AssociateContactWithCompany(createdCompany, createdContact); // replace with your actual method

        // Delay is required to allow time for new data to be searchable.
        await Task.Delay(7000);

        var associations = _companyApi.GetAssociations(createdCompany);

        using (new AssertionScope())
        {
            associations.Should().NotBeNull("Expected to retrieve the associations set for the company.");
            associations.Associations.AssociatedContacts.Should().Contain(createdContact.Id.Value,
                "The associated contact ID should be present in the retrieved associations.");
        }
    }

    public void Dispose()
    {
        foreach (var companyId in _companiesToCleanup)
        {
            _companyApi.Delete(companyId);
        }

        foreach (var contactId in _contactsToCleanup)
        {
            _contactApi.Delete(contactId);
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
        var createdCompany = _companyApi.Create(newCompany);

        createdCompany.Should().NotBeNull();
        createdCompany.Id.Should().HaveValue();
        _companiesToCleanup.Add(createdCompany.Id.Value);

        return createdCompany;
    }

    private ContactHubSpotModel CreateTestContact(string? email = null, string? firstname = null, string? lastname = null)
    {
        var newContact = new ContactHubSpotModel
        {
            Email = email,
            FirstName = firstname,
            LastName = lastname
        };

        var createdContact = _contactApi.Create(newContact);

        createdContact.Should().NotBeNull();
        createdContact.Id.Should().HaveValue();
        _contactsToCleanup.Add(createdContact.Id.Value);

        return createdContact;
    }

    private void AssociateContactWithCompany(CompanyHubSpotModel company, ContactHubSpotModel contact)
    {
        try
        {
            _hubSpotApi.Associations.AssociationToObject(HubSpotObjectIds.Company, company.Id.Value.ToString(),
                HubSpotObjectIds.Contact, contact.Id.Value.ToString());
        }
        catch (Exception ex)
        {
            // handle the error as needed
            Console.WriteLine($"Error while associating contact {contact.Id} with company {company.Id}: {ex.Message}");
        }
    }
}