using FluentAssertions;
using FluentAssertions.Execution;
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

        var createdContact = CreateTestContact(expectedEmail, expectedFirstName, expectedLastName, expectedCompany, expectedPhone);

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
        var createdContact = CreateTestContact();
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
        var createdContact = CreateTestContact();
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
        var createdContact = CreateTestContact();
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
}