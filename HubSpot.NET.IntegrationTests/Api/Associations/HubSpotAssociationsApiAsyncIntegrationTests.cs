using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.Contact.Dto;

namespace HubSpot.NET.IntegrationTests.Api.Associations;

public sealed class HubSpotAssociationsApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact]
    public async Task CreateAssociationAndFetchToObject()
    {
        var expectedCompany = await RecreateTestCompanyAsync();
        var expectedContact = await RecreateTestContactAsync();

        var expectedObjectType = "Company";
        var expectedObjectId = expectedCompany.Id.Value.ToString();
        var expectedToObjectType = "Contact";
        var expectedToObjectId = expectedContact.Id.Value.ToString();

        await AssociationsApi.AssociationToObjectAsync(expectedObjectType, expectedObjectId, expectedToObjectType,
            expectedToObjectId);

        // Need to wait for data to be searchable.
        await Task.Delay(7000);

        var fetchedContact = await ContactApi.GetByIdAsync<ContactHubSpotModel>(expectedContact.Id.Value);

        using (new AssertionScope())
        {
            fetchedContact.AssociatedCompanyId.Should().Be(expectedCompany.Id.Value,
                "The associated company ID should be present in the retrieved contact");
        }
    }

    [Fact(Skip = "Depends on the implementation of label API handling")]
    public async Task CreateAssociationByLabelAndFetchToObject()
    {
        var expectedCompany = await RecreateTestCompanyAsync();
        var expectedContact = await RecreateTestContactAsync();

        var expectedObjectType = "Company";
        var expectedObjectId = expectedCompany.Id.Value.ToString();
        var expectedToObjectType = "Contact";
        var expectedToObjectId = expectedContact.Id.Value.ToString();
        var expectedAssociationCategory = "YOUR_ASSOCIATION_CATEGORY";
        var expectedAssociationTypeId = 0; // replace with your actual association type ID

        await AssociationsApi.AssociationToObjectByLabelAsync(expectedObjectType, expectedObjectId,
            expectedToObjectType,
            expectedToObjectId, expectedAssociationCategory, expectedAssociationTypeId);

        // Need to wait for data to be searchable.
        await Task.Delay(7000);

        var fetchedContact = await ContactApi.GetByIdAsync<ContactHubSpotModel>(expectedContact.Id.Value);

        using (new AssertionScope())
        {
            fetchedContact.AssociatedCompanyId.Should().Be(expectedCompany.Id.Value,
                "The associated company ID should be present in the retrieved contact");
        }
    }
}