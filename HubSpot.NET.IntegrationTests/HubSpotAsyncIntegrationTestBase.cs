using HubSpot.NET.Api;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.ContactList.Dto;
using HubSpot.NET.Api.CustomEvent.Dto;
using HubSpot.NET.Api.Deal.Dto;
using HubSpot.NET.Api.LineItem.DTO;
using HubSpot.NET.Api.Properties.Dto;
using HubSpot.NET.Api.Schemas;
using HubSpot.NET.Core;
using SearchRequestFilter = HubSpot.NET.Api.SearchRequestFilter;
using SearchRequestFilterGroup = HubSpot.NET.Api.SearchRequestFilterGroup;

namespace HubSpot.NET.IntegrationTests;

[Collection("Integration tests collection")]
public abstract class HubSpotAsyncIntegrationTestBase : HubSpotIntegrationTestSetup
{
    protected async Task<CompanyHubSpotModel> RecreateTestCompanyAsync(string name = "Test Company",
        string country = "Test Country", string website = "www.testwebsite.com")
    {
        var existingCompany = (await CompanyApi.SearchAsync<CompanyHubSpotModel>(new SearchRequestOptions
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
        })).Results.FirstOrDefault();

        if (existingCompany != null)
        {
            await CompanyApi.DeleteAsync(existingCompany.Id.Value);
            CompaniesToCleanup.Remove(existingCompany.Id.Value);
        }

        var newCompany = new CompanyHubSpotModel
        {
            Name = name,
            Country = country,
            Website = website
        };
        var createdCompany = await CompanyApi.CreateAsync(newCompany);
        CompaniesToCleanup.Add(createdCompany.Id.Value);

        return createdCompany;
    }

    protected async Task<ContactHubSpotModel> RecreateTestContactAsync(string email = "test@email.com",
        string firstname = "Test Firstname",
        string lastname = "Test Lastname", string company = "Test Company", string phone = "1234567890")
    {
        var existingContact = await ContactApi.GetByEmailAsync<ContactHubSpotModel>(email);

        if (existingContact != null)
        {
            await ContactApi.DeleteAsync(existingContact.Id.Value);
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

        var createdContact = await ContactApi.CreateAsync(newContact);

        ContactsToCleanup.Add(createdContact.Id.Value);

        return createdContact;
    }

    protected async Task AssociateContactWithCompanyAsync(CompanyHubSpotModel company, ContactHubSpotModel contact)
    {
        try
        {
            await HubSpotApi.Associations.AssociationToObjectAsync(HubSpotObjectIds.Company,
                company.Id.Value.ToString(),
                HubSpotObjectIds.Contact, contact.Id.Value.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while associating contact {contact.Id} with company {company.Id}: {ex.Message}");
        }
    }

    protected async Task<ContactListModel> RecreateTestContactListAsync(string name = "Test Contact List")
    {
        var contactLists = (await ContactListApi.GetContactListsAsync()).Lists;

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
            createdContactList = await ContactListApi.CreateStaticContactListAsync(newContactList.Name);
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

    protected async Task<CompanyPropertyHubSpotModel> RecreateTestCompanyPropertyAsync(string name = "TestPropertyName",
        string type = "string", string fieldType = "text", string groupName = "TestGroup", string label = "TestLabel")
    {
        var allProperties = (await CompanyPropertiesApi.GetAllAsync()).Results;

        var existingProperty = allProperties.Find(p => p.Name == name);

        if (existingProperty != null)
        {
            await CompanyPropertiesApi.DeleteAsync(existingProperty.Name);
        }

        var newProperty = new CompanyPropertyHubSpotModel
        {
            Name = name,
            Type = type,
            FieldType = fieldType,
            GroupName = groupName,
            Label = label
        };

        var createdProperty = await CompanyPropertiesApi.CreateAsync(newProperty);

        CompanyPropertiesToCleanup.Add(createdProperty.Name);

        return createdProperty;
    }

    protected async Task<DealHubSpotModel> CreateTestDeal(string? dealName = null, double? amount = null)
    {
        dealName ??= "Test Deal " + Guid.NewGuid().ToString("N");

        var newDeal = new DealHubSpotModel
        {
            Name = dealName,
            Amount = amount ?? 1000  // Default amount if not provided
        };

        var createdDeal = await DealApi.CreateAsync(newDeal);

        DealsToCleanup.Add(createdDeal.Id.Value);  // Add deal ID to cleanup list

        return createdDeal;
    }

    protected async Task<(LineItemCreateOrUpdateRequest, LineItemGetResponse)> CreateTestLineItem(string? lineItemName = null)
    {
        lineItemName ??= "Test Line Item " + Guid.NewGuid();
        var newLineItem = new LineItemCreateOrUpdateRequest
        {
            Properties = new LineItemPropertiesHubSpotModel
            {
                Name = lineItemName,
                Price = 100
            }
        };
        var itemGetResponse =
            await LineItemApi.CreateAsync<LineItemCreateOrUpdateRequest, LineItemGetResponse>(newLineItem);

        LineItemsToCleanup.Add(itemGetResponse.Id.Value);
        return (newLineItem, itemGetResponse);
    }

    protected async Task<EventDefinition> GetOrCreateTestEventDefinition(string eventName = "test_event1",
        string primaryObject = "CONTACT")
    {
        var existingEventDef = await CustomEventApi.GetByNameAsync<EventDefinition>(eventName);

        if (existingEventDef != null)
        {
            return existingEventDef;
        }

        var newEventDefinition = new EventDefinition
        {
            Label = new SchemasLabelsModel() { Singular = eventName, Plural = "Test Events" },
            Name = "testevent12345",
            PrimaryObject = primaryObject,
            Description = "test description"           
        };

        var createdEventDefinition = await CustomEventApi.CreateAsync(newEventDefinition);

        return createdEventDefinition;
    }
}