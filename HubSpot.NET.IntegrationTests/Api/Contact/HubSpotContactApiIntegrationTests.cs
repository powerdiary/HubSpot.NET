using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.Contact;

public sealed class HubSpotContactApiIntegrationTests : HubSpotIntegrationTestBase
{
    [Fact]
    public void CreateContact()
    {
        const string expectedEmail = "testemail@test.com";
        const string expectedFirstName = "TestFirstName";
        const string expectedLastName = "TestLastName";
        const string expectedCompany = "TestCompany";
        const string expectedPhone = "1234567890";

        var createdContact = RecreateTestContact(expectedEmail, expectedFirstName, expectedLastName, expectedCompany, expectedPhone);

        using (new AssertionScope())
        {
            createdContact.Should().NotBeNull();
            createdContact.Email.Should().Be(expectedEmail);
            createdContact.FirstName.Should().Be(expectedFirstName);
            createdContact.LastName.Should().Be(expectedLastName);
            createdContact.Company.Should().Be(expectedCompany);
            createdContact.Phone.Should().Be(expectedPhone);
        }
    }

    [Fact]
    public void GetContactById()
    {
        var createdContact = RecreateTestContact();
        var contactById = ContactApi.GetById<ContactHubSpotModel>(createdContact.Id.Value);

        using (new AssertionScope())
        {
            contactById.Should().NotBeNull();
            contactById.Id.Should().Be(createdContact.Id);
            contactById.FirstName.Should().Be(createdContact.FirstName);
            contactById.LastName.Should().Be(createdContact.LastName);
            contactById.Email.Should().Be(createdContact.Email);
            contactById.Phone.Should().Be(createdContact.Phone);
            contactById.Company.Should().Be(createdContact.Company);
        }
    }

    [Fact]
    public void UpdateContact()
    {
        var createdContact = RecreateTestContact();
        createdContact.FirstName = "UpdatedFirstName";
        createdContact.LastName = "UpdatedLastName";
        createdContact.Email = "updatedemail@test.com";
        createdContact.Phone = "0987654321";
        createdContact.Company = "UpdatedCompany";

        ContactApi.Update(createdContact);

        var retrievedContact = ContactApi.GetById<ContactHubSpotModel>(createdContact.Id.Value);

        using (new AssertionScope())
        {
            retrievedContact.Should().NotBeNull();
            retrievedContact.FirstName.Should().Be(createdContact.FirstName);
            retrievedContact.LastName.Should().Be(createdContact.LastName);
            retrievedContact.Email.Should().Be(createdContact.Email);
            retrievedContact.Phone.Should().Be(createdContact.Phone);
            retrievedContact.Company.Should().Be(createdContact.Company);
        }
    }

    [Fact]
    public void DeleteContact()
    {
        var createdContact = RecreateTestContact();
        ContactApi.Delete(createdContact.Id.Value);

        var retrievedContact = ContactApi.GetById<ContactHubSpotModel>(createdContact.Id.Value);
        retrievedContact.Should().BeNull();
    }

    // NOTE: There's a difference in behavior compared to the Companies case,
    // where equivalent code DOES NOT throw an exception.
    // Perhaps we should make all or none of the methods throw exceptions.
    [Fact]
    public void DeleteContact_WhenContactDoesNotExist_ShouldThrowException()
    {
        var act = () => ContactApi.Delete(0);
        act.Should().Throw<HubSpotException>();
    }

    [Fact]
    public void GetContactByEmail()
    {
        var createdContact = RecreateTestContact("testemail@test.com");
        var contactByEmail = ContactApi.GetByEmail<ContactHubSpotModel>(createdContact.Email);

        using (new AssertionScope())
        {
            contactByEmail.Should().NotBeNull();
            contactByEmail.Id.Should().Be(createdContact.Id);
            contactByEmail.FirstName.Should().Be(createdContact.FirstName);
            contactByEmail.LastName.Should().Be(createdContact.LastName);
            contactByEmail.Email.Should().Be(createdContact.Email);
        }
    }

    /*
    NOTE: The GetByUserToken method in the HubSpotContactApi cannot be integration tested
    because the User Token (contactUtk) is not retrieved when creating or updating a contact
    through the HubSpot API in our current setup. User Tokens are usually generated through
    HubSpot's tracking code when a user visits the website and accepts cookies. This test
    method should be revisited once we have the capacity to programmatically generate or
    retrieve User Tokens in our tests.
    */

    [Fact]
    public async Task ListContacts()
    {
        var firstCreatedContact = RecreateTestContact("firstcontact@test.com");
        var secondCreatedContact = RecreateTestContact("secondcontact@test.com");

        await Task.Delay(10000);

        var contactList = ContactApi.List<ContactHubSpotModel>(new ContactSearchRequestOptions
        {
            PropertiesToInclude = new List<string> { "email" }
        });

        using (new AssertionScope())
        {
            contactList.Should().NotBeNull();
            var contacts = contactList.Contacts.Where(contact =>
                contact.Email == firstCreatedContact.Email ||
                contact.Email == secondCreatedContact.Email);
            contacts.Should().HaveCount(2);
        }
    }

    [Fact]
    public void BatchCreateContacts()
    {
        var firstCreatedContact = RecreateTestContact("firstcontact@test.com");
        var secondCreatedContact = RecreateTestContact("secondcontact@test.com");
        var contactsToCreate = new List<ContactHubSpotModel> { firstCreatedContact, secondCreatedContact };

        ContactApi.Batch(contactsToCreate);

        using(new AssertionScope())
        {
            ContactApi.GetByEmail<ContactHubSpotModel>(firstCreatedContact.Email).Should().NotBeNull();
            ContactApi.GetByEmail<ContactHubSpotModel>(secondCreatedContact.Email).Should().NotBeNull();
        }
    }

    [Fact]
    public async Task RecentlyUpdatedContacts()
    {
        var firstCreatedContact = RecreateTestContact("firstcontact@test.com");
        var secondCreatedContact = RecreateTestContact("secondcontact@test.com");

    	var updatedContact1 = UpdateTestContact(firstCreatedContact.Id.Value, "UpdatedFirstName1");
    	var updatedContact2 = UpdateTestContact(secondCreatedContact.Id.Value, "UpdatedFirstName2");

        await Task.Delay(10000);

        var recentContacts = ContactApi.RecentlyUpdated<ContactHubSpotModel>().Contacts;

        using(new AssertionScope())
        {
            recentContacts.Should().NotBeNullOrEmpty();
            recentContacts.Should().Contain(contact => contact.Id == updatedContact1.Id);
            recentContacts.Should().Contain(contact => contact.Id == updatedContact2.Id);
        }
    }

    [Fact]
    public async Task SearchContacts()
    {
        var testContact1 = RecreateTestContact("john.doe@test.com", "John", "Doe");
        var testContact2 = RecreateTestContact("jane.doe@test.com", "Jane", "Doe");
        var testContact3 = RecreateTestContact("bob.ross@test.com", "Bob", "Ross");

        await Task.Delay(10000);

        var filters = new SearchRequestFilter
        {
            PropertyName = "lastname", // Assuming "Doe" refers to last name
            Operator = SearchRequestFilterOperatorType.EqualTo,
            Value = "Doe"
        };

        var filterGroup = new SearchRequestFilterGroup { Filters = new List<SearchRequestFilter> { filters } };

        var searchResults = ContactApi.Search<ContactHubSpotModel>(new SearchRequestOptions
        {
            PropertiesToInclude = new List<string> { "email", "firstName", "lastName" },
            FilterGroups = new List<SearchRequestFilterGroup> { filterGroup }
        }).Results;

        using(new AssertionScope())
        {
            searchResults.Should().NotBeEmpty();
            searchResults.Count.Should().Be(2);
            searchResults.Select(c => c.Email).Should().Contain(new List<string> { testContact1.Email, testContact2.Email });
            searchResults.Select(c => c.Email).Should().NotContain(testContact3.Email);
        }
    }

    [Fact]
    public async Task RecentlyCreatedContactsTest()
    {
        var newContact1 = RecreateTestContact("newContact1@test.com");
        var newContact2 = RecreateTestContact("newContact2@test.com");

        await Task.Delay(10000);

        var recentContacts = ContactApi.RecentlyCreated<ContactHubSpotModel>().Contacts;

        using(new AssertionScope())
        {
            recentContacts.Should().NotBeNullOrEmpty();
            recentContacts.Should().Contain(contact => contact.Id == newContact1.Id);
            recentContacts.Should().Contain(contact => contact.Id == newContact2.Id);
        }
    }

    private ContactHubSpotModel UpdateTestContact(long id, string updatedFirstName = "UpdatedFirstName")
    {
        var testContact = ContactApi.GetById<ContactHubSpotModel>(id);

        testContact.FirstName = updatedFirstName;
        testContact.LastName = "UpdatedLastName";
        testContact.Phone = "0987654321";
        testContact.Company = "UpdatedCompany";

        ContactApi.Update(testContact);

        return testContact;
    }
}