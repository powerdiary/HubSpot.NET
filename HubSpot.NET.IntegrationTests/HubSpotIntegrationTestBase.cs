using HubSpot.NET.Api;
using HubSpot.NET.Api.Associations;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.ContactList;
using HubSpot.NET.Api.ContactList.Dto;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Api.Properties;
using HubSpot.NET.Api.Properties.Dto;
using HubSpot.NET.Core;
using Microsoft.Extensions.Configuration;
using SearchRequestFilter = HubSpot.NET.Api.SearchRequestFilter;
using SearchRequestFilterGroup = HubSpot.NET.Api.SearchRequestFilterGroup;

namespace HubSpot.NET.IntegrationTests;

public abstract class HubSpotIntegrationTestBase : IDisposable
{
    private readonly string? ApiKey;
    protected readonly HubSpotAssociationsApi AssociationsApi;
    protected readonly HubSpotCompanyApi CompanyApi;
    protected readonly HubSpotContactApi ContactApi;
    protected readonly HubSpotContactListApi ContactListApi;
    protected readonly HubSpotCompaniesPropertiesApi CompanyPropertiesApi;
    protected readonly HubSpotCustomObjectApi CustomObjectApi;

    private readonly HubSpotApi _hubSpotApi;

    private readonly IList<long> _companiesToCleanup = new List<long>();
    private readonly IList<long> _contactsToCleanup = new List<long>();
    private readonly IList<long> _contactListToCleanup = new List<long>();
    private readonly IList<string> _companyPropertiesToCleanup = new List<string>();

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
        AssociationsApi = new HubSpotAssociationsApi(client);
        CompanyApi = new HubSpotCompanyApi(client);
        ContactApi = new HubSpotContactApi(client);
        ContactListApi = new HubSpotContactListApi(client);
        CompanyPropertiesApi = new HubSpotCompaniesPropertiesApi(client);
        CustomObjectApi = new HubSpotCustomObjectApi(client, AssociationsApi);
        _hubSpotApi = new HubSpotApi(ApiKey);
    }

    protected CompanyHubSpotModel RecreateTestCompany(string name = "Test Company", string country = "Test Country", string website = "www.testwebsite.com")
    {
        var existingCompany = CompanyApi.Search<CompanyHubSpotModel>(new SearchRequestOptions
        {
            FilterGroups = new List<SearchRequestFilterGroup>
            {
                new()
                {
                    Filters = new List<SearchRequestFilter>
                    {
                        new() { Operator = SearchRequestFilterOperatorType.EqualTo, Value = name, PropertyName = "name" }
                    }
                }
            }
        }).Results.FirstOrDefault();

        if (existingCompany != null)
        {
            CompanyApi.Delete(existingCompany.Id.Value);
            _companiesToCleanup.Remove(existingCompany.Id.Value);
        }

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

    protected ContactListModel RecreateTestContactList(string name = "Test Contact List")
    {
        var contactLists = ContactListApi.GetContactLists().Lists;

        var existingList = contactLists.Find(cl => cl.Name.Equals(name));

        if (existingList != null)
        {
            _contactListToCleanup.Add(existingList.ListId);
            return existingList;
        }

        var newContactList = new ContactListModel
        {
            Name = name,
            Dynamic = false
        };

        ContactListModel createdContactList;

        try
        {
            createdContactList = ContactListApi.CreateStaticContactList(newContactList.Name);
            _contactListToCleanup.Add(createdContactList.ListId);
        }
        catch (HubSpotException ex)
        {
            throw new Exception(
                "Error creating contact list. If this is due to the contact list name not being unique, " +
                "please ensure you have removed any unused contact lists or use a unique name. " +
                "Complete extraction of contact lists for the name check isn't currently possible until " +
                "the feature with 'offset=' parameter is implemented in this library.", ex);
        }

        return createdContactList;
    }

    protected CompanyPropertyHubSpotModel RecreateTestCompanyProperty(string name = "TestPropertyName",
        string type = "string", string fieldType = "text", string groupName = "TestGroup", string label = "TestLabel")
    {
        var allProperties = CompanyPropertiesApi.GetAll().Results;

        var existingProperty = allProperties.Find(p => p.Name == name);

        if (existingProperty != null)
        {
            CompanyPropertiesApi.Delete(existingProperty.Name);
        }

        var newProperty = new CompanyPropertyHubSpotModel
        {
            Name = name,
            Type = type,
            FieldType = fieldType,
            GroupName = groupName,
            Label = label
        };

        var createdProperty = CompanyPropertiesApi.Create(newProperty);

        _companyPropertiesToCleanup.Add(createdProperty.Name);

        return createdProperty;
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
        if (_isDisposed || !disposing)
            return;

        CleanCompanies();
        CleanContacts();
        CleanContactLists();
        CleanCompanyProperties();

        _isDisposed = true;
    }

    private void CleanCompanies()
    {
        foreach (var companyId in _companiesToCleanup)
        {
            CompanyApi.Delete(companyId);
        }
    }

    private void CleanContacts()
    {
        foreach (var contactId in _contactsToCleanup)
        {
            TryAction(() => ContactApi.Delete(contactId));
        }
    }

    private void CleanContactLists()
    {
        foreach (var contactListId in _contactListToCleanup)
        {
            TryAction(() => ContactListApi.DeleteContactList(contactListId));
        }
    }

    private void CleanCompanyProperties()
    {
        foreach (var propertyName in _companyPropertiesToCleanup)
        {
            try
            {
                CompanyPropertiesApi.Delete(propertyName);
            }
            catch
            {
                // ignored
            }
        }
    }

    private void TryAction(Action action)
    {
        try
        {
            action();
        }
        catch
        {
            // ignored
        }
    }

    ~HubSpotIntegrationTestBase()
    {
        Dispose(false);
    }
    #endregion
}