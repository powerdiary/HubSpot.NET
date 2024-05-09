using FluentAssertions;
using FluentAssertions.Execution;
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

        // Need to wait for data to be searchable.
        await Task.Delay(7000);

        var fetchedContact = ContactApi.GetById<ContactHubSpotModel>(expectedContact.Id.Value);

        using (new AssertionScope())
        {
            fetchedContact.AssociatedCompanyId.Should().Be(expectedCompany.Id.Value,
                "The associated company ID should be present in the retrieved contact");
        }
    }

    [Fact(Skip = "Depends on the implementation of label API handling")]
    public async Task CreateAssociationByLabelAndFetchToObject()
    {
        var expectedCompany = CreateTestCompany();
        var expectedContact = RecreateTestContact();

        var expectedObjectType = "Company";
        var expectedObjectId = expectedCompany.Id.Value.ToString();
        var expectedToObjectType = "Contact";
        var expectedToObjectId = expectedContact.Id.Value.ToString();
        var expectedAssociationCategory = "YOUR_ASSOCIATION_CATEGORY";
        var expectedAssociationTypeId = 0; // replace with your actual association type ID

        AssociationsApi.AssociationToObjectByLabel(expectedObjectType, expectedObjectId, expectedToObjectType,
            expectedToObjectId, expectedAssociationCategory, expectedAssociationTypeId);

        // Need to wait for data to be searchable.
        await Task.Delay(7000);

        var fetchedContact = ContactApi.GetById<ContactHubSpotModel>(expectedContact.Id.Value);

        using (new AssertionScope())
        {
            fetchedContact.AssociatedCompanyId.Should().Be(expectedCompany.Id.Value,
                "The associated company ID should be present in the retrieved contact");
        }
    }
}