using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.ContactList.Dto;

namespace HubSpot.NET.IntegrationTests.Api.ContactList;

public class HubSpotContactListApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact]
    public async Task CreateStaticContactList()
    {
        var expectedContactListName = "StaticTestList";
        var createdContactList = await RecreateTestContactListAsync(expectedContactListName);

        using (new AssertionScope())
        {
            createdContactList.Should().NotBeNull();
            createdContactList.Name.Should().Be(expectedContactListName);
            createdContactList.Dynamic.Should().Be(false);
        }
    }

    [Fact]
    public async Task GetContactListById()
    {
        var createdContactList = await RecreateTestContactListAsync("StaticTestList");
        var contactListById = await ContactListApi.GetContactListByIdAsync(createdContactList.ListId);

        using (new AssertionScope())
        {
            contactListById.Should().NotBeNull();
            contactListById.ListId.Should().Be(createdContactList.ListId);
            contactListById.Name.Should().Be(createdContactList.Name);
        }
    }

    [Fact]
    public async Task ContactListById_WhenDoesNotExist_ShouldBeNull()
    {
        var nonExistingId = 0;

        var contactListById = await ContactListApi.GetContactListByIdAsync(nonExistingId);

        using (new AssertionScope())
        {
            contactListById.Should().BeNull();
        }
    }

    [Fact]
    public async Task AddContactsToList()
    {
        var (contact1, contact2, contactList) = await CreateContactsAndList();

        var additionResult =
            await ContactListApi.AddContactsToListAsync(contactList.ListId, new[] { contact1.Id.Value, contact2.Id.Value });

        using (new AssertionScope())
        {
            additionResult.UpdatedContactIds.Should().Contain(new[] { contact1.Id.Value, contact2.Id.Value });
        }
    }

    [Fact]
    public async Task RemoveContactsFromList()
    {
        var (contact1, contact2, contactList) = await CreateContactsAndList();

        await ContactListApi.AddContactsToListAsync(contactList.ListId, new[] { contact1.Id.Value, contact2.Id.Value });

        var removalResult =
            await ContactListApi.RemoveContactsFromListAsync(contactList.ListId, new[] { contact1.Id.Value, contact2.Id.Value });

        using (new AssertionScope())
        {
            removalResult.UpdatedContactIds.Should().Contain(new[] { contact1.Id.Value, contact2.Id.Value });
        }
    }

    [Fact]
    public async Task DeleteContactList()
    {
        var createdContactList = await RecreateTestContactListAsync("StaticTestList");
        await ContactListApi.DeleteContactListAsync(createdContactList.ListId);

        var deletedContactList = await ContactListApi.GetContactListByIdAsync(createdContactList.ListId);

        using (new AssertionScope())
        {
            deletedContactList.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetContactLists()
    {
        var createdContactList1 = await RecreateTestContactListAsync("TestList1");
        var createdContactList2 = await RecreateTestContactListAsync("TestList2");

        // to make data searchable
        await Task.Delay(10000);

        var contactLists = (await ContactListApi.GetContactListsAsync()).Lists;

        using (new AssertionScope())
        {
            contactLists.Should().NotBeNull();
            contactLists.Should().Contain(c => c.ListId == createdContactList1.ListId);
            contactLists.Should().Contain(c => c.ListId == createdContactList2.ListId);
        }
    }

    // Note: The HubSpotContactListApi currently only allows for the creation of static lists,
    // which is why these tests are designed around them. If future updates to the library
    // add the ability to create dynamic lists, these tests should be revised to account for that.
    [Fact]
    public async Task GetStaticContactLists()
    {
        var createdContactList1 = await RecreateTestContactListAsync("StaticTestList1");
        var createdContactList2 = await RecreateTestContactListAsync("StaticTestList2");

        // to make data searchable
        await Task.Delay(10000);

        var contactLists = (await ContactListApi.GetStaticContactListsAsync()).Lists;

        using (new AssertionScope())
        {
            contactLists.Should().NotBeNull();
            contactLists.Should().Contain(c => c.ListId == createdContactList1.ListId);
            contactLists.Should().Contain(c => c.ListId == createdContactList2.ListId);
        }
    }

    private async Task<(ContactHubSpotModel contact1, ContactHubSpotModel contact2, ContactListModel contactList)>
        CreateContactsAndList()
    {
        var createdContact1 = await RecreateTestContactAsync("createdcontact1@test.com", "FirstName1", "LastName1", "TestCompany1",
            "1234567891");
        var createdContact2 = await RecreateTestContactAsync("createdcontact2@test.com", "FirstName2", "LastName2", "TestCompany2",
            "1234567892");
        var createdContactList = await RecreateTestContactListAsync("StaticTestList");

        return (createdContact1, createdContact2, createdContactList);
    }
}