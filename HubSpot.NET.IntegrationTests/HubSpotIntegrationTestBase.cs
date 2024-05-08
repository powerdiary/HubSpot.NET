using FluentAssertions;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using Microsoft.Extensions.Configuration;

namespace HubSpot.NET.IntegrationTests;

public abstract class HubSpotIntegrationTestBase: IDisposable
{
    private readonly string? ApiKey;
    protected readonly HubSpotCompanyApi CompanyApi;
    private readonly HubSpotContactApi _contactApi;
    private readonly HubSpotApi _hubSpotApi;

    private readonly IList<long> _companiesToCleanup;
    private readonly IList<long> _contactsToCleanup;

    protected HubSpotIntegrationTestBase()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
                optional: true)
            .Build();

        ApiKey = configuration["HubSpot:PrivateAppKey"];

        var client = new HubSpotBaseClient(ApiKey);
        CompanyApi = new HubSpotCompanyApi(client);
        _contactApi = new HubSpotContactApi(client);
        _hubSpotApi = new HubSpotApi(ApiKey);

        _companiesToCleanup = new List<long>();
        _contactsToCleanup = new List<long>();
    }

    protected CompanyHubSpotModel CreateTestCompany(string? name = null, string? country = null, string? website = null)
    {
        var newCompany = new CompanyHubSpotModel
        {
            Name = name,
            Country = country,
            Website = website
        };
        var createdCompany = CompanyApi.Create(newCompany);

        createdCompany.Should().NotBeNull();
        createdCompany.Id.Should().HaveValue();
        _companiesToCleanup.Add(createdCompany.Id.Value);

        return createdCompany;
    }

    protected ContactHubSpotModel CreateTestContact(string? email = null, string? firstname = null, string? lastname = null)
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

    protected void AssociateContactWithCompany(CompanyHubSpotModel company, ContactHubSpotModel contact)
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

    public void Dispose()
    {
        foreach (var companyId in _companiesToCleanup)
        {
            CompanyApi.Delete(companyId);
        }

        foreach (var contactId in _contactsToCleanup)
        {
            _contactApi.Delete(contactId);
        }
    }
}