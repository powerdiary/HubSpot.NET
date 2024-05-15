using FluentAssertions;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.CustomObject;

public sealed class HubSpotCustomObjectApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact]
    public async Task List_GivenUnknownObjectId_ShouldThrowException()
    {
        const string idForCustomObject = "unknown_known_custom_object_id";
        var opts = new ListRequestOptions();

        var act = async () => await CustomObjectApi.ListAsync<CustomObjectHubSpotModel>(idForCustomObject, opts);
        await act.Should().ThrowAsync<HubSpotException>()
            .WithMessage("*Unable to infer object type from: unknown_known_custom_object_id*");
    }

    [Fact(Skip = "Setup your own CustomObject and provide an ID for it.")]
    public async Task List_GivenExistingObject_ShouldGetResults()
    {
        // Arrange a custom object at https://app.hubspot.com/ with a property
        const string customProperty = "machine_name";
        const string idForCustomObject = "2-29369202";

        var opts = new ListRequestOptions
        {
            Limit = 2,
            PropertiesToInclude = new List<string> { "hs_created_by_user_id", customProperty }
        };

        var customObjectList = (await CustomObjectApi.ListAsync<CustomObjectHubSpotModel>(idForCustomObject, opts)).Results;

        customObjectList.Should().NotBeNull();
    }
}