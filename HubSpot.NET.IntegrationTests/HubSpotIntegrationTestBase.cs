using HubSpot.NET.Api;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.ContactList.Dto;
using HubSpot.NET.Api.Properties.Dto;
using HubSpot.NET.Core;
using SearchRequestFilter = HubSpot.NET.Api.SearchRequestFilter;
using SearchRequestFilterGroup = HubSpot.NET.Api.SearchRequestFilterGroup;

namespace HubSpot.NET.IntegrationTests;

[Collection("Integration tests collection")]
public abstract class HubSpotIntegrationTestBase : HubSpotIntegrationTestSetup
{
    protected CompanyHubSpotModel RecreateTestCompany(string name = "Test Company", string country = "Test Country",
        string website = "www.testwebsite.com")
    {
        var existingCompany = CompanyApi.Search<CompanyHubSpotModel>(new SearchRequestOptions
        {
            FilterGroups = new List<SearchRequestFilterGroup>
            {
                new()
                {
                    Filters = new List<SearchRequestFilter>
                    {
                        new()
                        {
                            Operator = SearchRequestFilterOperatorType.EqualTo, Value = name, PropertyName = "name"
                        }
                    }
                }
            }
        }).Results.FirstOrDefault();

        if (existingCompany != null)
        {
            CompanyApi.Delete(existingCompany.Id.Value);
            CompaniesToCleanup.Remove(existingCompany.Id.Value);
        }

        var newCompany = new CompanyHubSpotModel
        {
            Name = name,
            Country = country,
            Website = website
        };
        var createdCompany = CompanyApi.Create(newCompany);
        CompaniesToCleanup.Add(createdCompany.Id.Value);

        return createdCompany;
    }

    protected ContactHubSpotModel RecreateTestContact(string email = "test@email.com",
        string firstname = "Test Firstname",
        string lastname = "Test Lastname", string company = "Test Company", string phone = "1234567890")
    {
        var existingContact = ContactApi.GetByEmail<ContactHubSpotModel>(email);

        if (existingContact != null)
        {
            ContactApi.Delete(existingContact.Id.Value);
            ContactsToCleanup.Remove(existingContact.Id.Value);
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

        ContactsToCleanup.Add(createdContact.Id.Value);

        return createdContact;
    }

    protected void AssociateContactWithCompany(CompanyHubSpotModel company, ContactHubSpotModel contact)
    {
        try
        {
            HubSpotApi.Associations.AssociationToObject(HubSpotObjectIds.Company, company.Id.Value.ToString(),
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
            ContactListToCleanup.Add(existingList.ListId);
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
            ContactListToCleanup.Add(createdContactList.ListId);
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

        CompanyPropertiesToCleanup.Add(createdProperty.Name);

        return createdProperty;
    }
}