using FluentAssertions;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using Microsoft.Extensions.Configuration;

namespace HubSpot.NET.IntegrationTests;

public abstract class HubSpotIntegrationTestBase : IDisposable
{
    private readonly string? ApiKey;
    protected readonly HubSpotCompanyApi CompanyApi;
    protected readonly HubSpotContactApi ContactApi;
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
        ContactApi = new HubSpotContactApi(client);
        _hubSpotApi = new HubSpotApi(ApiKey);

        _companiesToCleanup = new List<long>();
        _contactsToCleanup = new List<long>();
    }

    protected CompanyHubSpotModel CreateTestCompany(string name = "Test Company", string country = "Test Country", string website = "www.testwebsite.com")
    {
        var newCompany = new CompanyHubSpotModel
        {
            Name = name,
            Country = country,
            Website = website
        };
        var createdCompany = CompanyApi.Create(newCompany);
        _companiesToCleanup.Add(createdCompany.Id.Value);

        return createdCompany;
    }

    protected ContactHubSpotModel CreateTestContact(string email = "test@email.com", string firstname = "Test Firstname",
        string lastname = "Test Lastname", string company = "Test Company", string phone = "1234567890")
    {
        var newContact = new ContactHubSpotModel
        {
            Email = email,
            FirstName = firstname,
            LastName = lastname,
            Company = company,
            Phone = phone
        };

        var createdContact = ContactApi.Create(newContact);
        _contactsToCleanup.Add(createdContact.Id.Value);

        return createdContact;
    }

    protected ContactHubSpotModel RecreateTestContact(string email = "test@email.com", string firstname = "Test Firstname",
        string lastname = "Test Lastname", string company = "Test Company", string phone = "1234567890")
    {
        var existingContact = ContactApi.GetByEmail<ContactHubSpotModel>(email);

        if (existingContact != null)
        {
            ContactApi.Delete(existingContact.Id.Value);
            _contactsToCleanup.Remove(existingContact.Id.Value);
        }

        var newContact = new ContactHubSpotModel
        {
            Email = email,
            FirstName = firstname,
            LastName = lastname,
            Company = company,
            Phone = phone
        };

        var createdContact = ContactApi.Create(newContact);

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
            Console.WriteLine($"Error while associating contact {contact.Id} with company {company.Id}: {ex.Message}");
        }
    }

    #region IDisposable Support
    private bool _isDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                foreach (var companyId in _companiesToCleanup)
                {
                    CompanyApi.Delete(companyId);
                }

                foreach (var contactId in _contactsToCleanup)
                {
                    try
                    {
                        ContactApi.Delete(contactId);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            _isDisposed = true;
        }
    }

    ~HubSpotIntegrationTestBase()
    {
        Dispose(false);
    }
    #endregion
}