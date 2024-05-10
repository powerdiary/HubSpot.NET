using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.ContactList.Dto;

namespace HubSpot.NET.IntegrationTests.Api.ContactList;

public class HubSpotContactListApiIntegrationTests : HubSpotIntegrationTestBase
{
    [Fact]
    public void CreateStaticContactList()
    {
        var expectedContactListName = "StaticTestList";
        var createdContactList = RecreateTestContactList(expectedContactListName);

        using (new AssertionScope())
        {
            createdContactList.Should().NotBeNull();
            createdContactList.Name.Should().Be(expectedContactListName);
            createdContactList.Dynamic.Should().Be(false);
        }
    }

    [Fact]
    public void GetContactListById()
    {
        var createdContactList = RecreateTestContactList("StaticTestList");
        var contactListById = ContactListApi.GetContactListById(createdContactList.ListId);

        using (new AssertionScope())
        {
            contactListById.Should().NotBeNull();
            contactListById.ListId.Should().Be(createdContactList.ListId);
            contactListById.Name.Should().Be(createdContactList.Name);
        }
    }

    [Fact]
    public void ContactListById_WhenDoesNotExist_ShouldBeNull()
    {
        var nonExistingId = 0;

        var contactListById = ContactListApi.GetContactListById(nonExistingId);

        using (new AssertionScope())
        {
            contactListById.Should().BeNull();
        }
    }

    [Fact]
    public void AddContactsToList()
    {
        var (contact1, contact2, contactList) = CreateContactsAndList();

        var additionResult =
            ContactListApi.AddContactsToList(contactList.ListId, new[] { contact1.Id.Value, contact2.Id.Value });

        using (new AssertionScope())
        {
            additionResult.UpdatedContactIds.Should().Contain(new[] { contact1.Id.Value, contact2.Id.Value });
        }
    }

    [Fact]
    public void RemoveContactsFromList()
    {
        var (contact1, contact2, contactList) = CreateContactsAndList();

        ContactListApi.AddContactsToList(contactList.ListId, new[] { contact1.Id.Value, contact2.Id.Value });

        var removalResult =
            ContactListApi.RemoveContactsFromList(contactList.ListId, new[] { contact1.Id.Value, contact2.Id.Value });

        using (new AssertionScope())
        {
            removalResult.UpdatedContactIds.Should().Contain(new[] { contact1.Id.Value, contact2.Id.Value });
        }
    }

    [Fact]
    public void DeleteContactList()
    {
        var createdContactList = RecreateTestContactList("StaticTestList");
        ContactListApi.DeleteContactList(createdContactList.ListId);

        var deletedContactList = ContactListApi.GetContactListById(createdContactList.ListId);

        using (new AssertionScope())
        {
            deletedContactList.Should().BeNull();
        }
    }

    private (ContactHubSpotModel contact1, ContactHubSpotModel contact2, ContactListModel contactList)
        CreateContactsAndList()
    {
        var createdContact1 = RecreateTestContact("createdcontact1@test.com", "FirstName1", "LastName1", "TestCompany1",
            "1234567891");
        var createdContact2 = RecreateTestContact("createdcontact2@test.com", "FirstName2", "LastName2", "TestCompany2",
            "1234567892");
        var createdContactList = RecreateTestContactList("StaticTestList");

        return (createdContact1, createdContact2, createdContactList);
    }
}