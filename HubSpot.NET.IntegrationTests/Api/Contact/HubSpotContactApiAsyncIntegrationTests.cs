using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.Contact;

public sealed class HubSpotContactApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact]
    public async Task CreateContact()
    {
        const string expectedEmail = "testemail@test.com";
        const string expectedFirstName = "TestFirstName";
        const string expectedLastName = "TestLastName";
        const string expectedCompany = "TestCompany";
        const string expectedPhone = "1234567890";

        var createdContact = await RecreateTestContactAsync(expectedEmail, expectedFirstName, expectedLastName, expectedCompany, expectedPhone);

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
    public async Task GetContactById()
    {
        var createdContact = await RecreateTestContactAsync();
        var contactById = await ContactApi.GetByIdAsync<ContactHubSpotModel>(createdContact.Id.Value);

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
     public async Task UpdateContact()
    {
        var createdContact = await RecreateTestContactAsync();
        createdContact.FirstName = "UpdatedFirstName";
        createdContact.LastName = "UpdatedLastName";
        createdContact.Email = "updatedemail@test.com";
        createdContact.Phone = "0987654321";
        createdContact.Company = "UpdatedCompany";

        await ContactApi.UpdateAsync(createdContact);

        var retrievedContact = await ContactApi.GetByIdAsync<ContactHubSpotModel>(createdContact.Id.Value);

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
     public async Task DeleteContact()
    {
        var createdContact = await RecreateTestContactAsync();
        await ContactApi.DeleteAsync(createdContact.Id.Value);

        var retrievedContact = await ContactApi.GetByIdAsync<ContactHubSpotModel>(createdContact.Id.Value);
        retrievedContact.Should().BeNull();
    }

    // NOTE: There's a difference in behavior compared to the Companies case,
    // where equivalent code DOES NOT throw an exception.
    // Perhaps we should make all or none of the methods throw exceptions.
    [Fact]
     public async Task DeleteContact_WhenContactDoesNotExist_ShouldThrowException()
    {
        var act = async () => await ContactApi.DeleteAsync(0);
        await act.Should().ThrowAsync<HubSpotException>();
    }

    [Fact]
     public async Task GetContactByEmail()
    {
        var createdContact = await RecreateTestContactAsync("testemail@test.com");
        var contactByEmail = await ContactApi.GetByEmailAsync<ContactHubSpotModel>(createdContact.Email);

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
        var firstCreatedContact = await RecreateTestContactAsync("firstcontact@test.com");
        var secondCreatedContact = await RecreateTestContactAsync("secondcontact@test.com");

        await Task.Delay(10000);

        var contactList = await ContactApi.ListAsync<ContactHubSpotModel>(new ContactSearchRequestOptions
        {
            PropertiesToInclude = new List<string> { "email" }
        });

        using (new AssertionScope())
        {
            contactList.Should().NotBeNull();

            if (contactList.Contacts.Count < 20)
            {
                var contacts = contactList.Contacts.Where(contact =>
                    contact.Email == firstCreatedContact.Email ||
                    contact.Email == secondCreatedContact.Email);

                contacts.Should().HaveCount(2);
            }
        }
    }

    [Fact]
     public async Task BatchCreateContacts()
    {
        var firstCreatedContact = await RecreateTestContactAsync("firstcontact@test.com");
        var secondCreatedContact = await RecreateTestContactAsync("secondcontact@test.com");
        var contactsToCreate = new List<ContactHubSpotModel> { firstCreatedContact, secondCreatedContact };

        await ContactApi.BatchAsync(contactsToCreate);

        using(new AssertionScope())
        {
            (await ContactApi.GetByEmailAsync<ContactHubSpotModel>(firstCreatedContact.Email)).Should().NotBeNull();
            (await ContactApi.GetByEmailAsync<ContactHubSpotModel>(secondCreatedContact.Email)).Should().NotBeNull();
        }
    }

    [Fact]
    public async Task RecentlyUpdatedContacts()
    {
        var firstCreatedContact = await RecreateTestContactAsync("firstcontact@test.com");
        var secondCreatedContact = await RecreateTestContactAsync("secondcontact@test.com");

    	var updatedContact1 = await UpdateTestContact(firstCreatedContact.Id.Value, "UpdatedFirstName1");
    	var updatedContact2 = await UpdateTestContact(secondCreatedContact.Id.Value, "UpdatedFirstName2");

        await Task.Delay(10000);

        var recentContacts = (await ContactApi.RecentlyUpdatedAsync<ContactHubSpotModel>()).Contacts;

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
        var guid1 = Guid.NewGuid().ToString("N");
        var guid2 = Guid.NewGuid().ToString("N");

        var testContact1 = await RecreateTestContactAsync($"{guid1}@test.com", "John", $"{guid1}");
        await RecreateTestContactAsync($"{guid2}@test.com", "Jane", $"{guid2}");

        await Task.Delay(10000);

        var filters = new SearchRequestFilter
        {
            PropertyName = "lastname",
            Operator = SearchRequestFilterOperatorType.EqualTo,
            Value = $"{guid1}"
        };

        var filterGroup = new SearchRequestFilterGroup { Filters = new List<SearchRequestFilter> { filters } };

        var searchResults = (await ContactApi.SearchAsync<ContactHubSpotModel>(new SearchRequestOptions
        {
            PropertiesToInclude = new List<string> { "email", "firstname", "lastname" },
            FilterGroups = new List<SearchRequestFilterGroup> { filterGroup }
        })).Results;

        using (new AssertionScope())
        {
            searchResults.Should().NotBeEmpty();
            searchResults.Should().ContainSingle(r => r.Email == testContact1.Email);
        }
    }

    [Fact]
    public async Task RecentlyCreatedContactsTest()
    {
        var newContact1 = await RecreateTestContactAsync("newContact1@test.com");
        var newContact2 = await RecreateTestContactAsync("newContact2@test.com");

        await Task.Delay(10000);

        var recentContacts = (await ContactApi.RecentlyCreatedAsync<ContactHubSpotModel>()).Contacts;

        using(new AssertionScope())
        {
            recentContacts.Should().NotBeNullOrEmpty();
            recentContacts.Should().Contain(contact => contact.Id == newContact1.Id);
            recentContacts.Should().Contain(contact => contact.Id == newContact2.Id);
        }
    }

    private async Task<ContactHubSpotModel> UpdateTestContact(long id, string updatedFirstName = "UpdatedFirstName")
    {
        var testContact = await ContactApi.GetByIdAsync<ContactHubSpotModel>(id);

        testContact.FirstName = updatedFirstName;
        testContact.LastName = "UpdatedLastName";
        testContact.Phone = "0987654321";
        testContact.Company = "UpdatedCompany";

        await ContactApi.UpdateAsync(testContact);

        return testContact;
    }
}