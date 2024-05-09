using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact.Dto;

namespace HubSpot.NET.IntegrationTests.Api.Associations;

public sealed class HubSpotAssociationsApiIntegrationTests : HubSpotIntegrationTestBase
{
    [Fact]
    public async Task CreateAssociationAndFetchToObject()
    {
        var expectedCompany = CreateTestCompany();
        var expectedContact = RecreateTestContact();

        var expectedObjectType = "Company";
        var expectedObjectId = expectedCompany.Id.Value.ToString();
        var expectedToObjectType = "Contact";
        var expectedToObjectId = expectedContact.Id.Value.ToString();

        AssociationsApi.AssociationToObject(expectedObjectType, expectedObjectId, expectedToObjectType,
            expectedToObjectId);

        // Need to wait for data to be searchable. Adjust delay based on your requirements/observations.
        await Task.Delay(7000);

        var fetchedCompany = CompanyApi.GetById<CompanyHubSpotModel>(expectedCompany.Id.Value);
        var fetchedContact = ContactApi.GetById<ContactHubSpotModel>(expectedContact.Id.Value);

        using (new AssertionScope())
        {
            fetchedContact.AssociatedCompanyId.Should().Be(expectedCompany.Id.Value,
                "The associated company ID should be present in the retrieved contact");

            fetchedCompany.Associations.Should().NotBeNull("Expected to retrieve associations set for the company.");
            fetchedCompany.Associations.AssociatedContacts.Should().Contain(expectedContact.Id.Value,
                "The associated contact ID should be present in the retrieved associations.");
        }
    }
}