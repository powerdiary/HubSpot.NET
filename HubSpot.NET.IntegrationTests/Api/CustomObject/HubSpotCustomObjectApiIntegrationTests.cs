using FluentAssertions;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.CustomObject;

public sealed class HubSpotCustomObjectApiIntegrationTests : HubSpotIntegrationTestBase
{
    [Fact]
    public void List_GivenUnknownObjectId_ShouldThrowException()
    {
        const string idForCustomObject = "unknown_known_custom_object_id";
        var opts = new ListRequestOptions();

        var act = () => CustomObjectApi.List<CustomObjectHubSpotModel>(idForCustomObject, opts).Results;
        act.Should().Throw<HubSpotException>()
            .WithMessage("*Unable to infer object type from: unknown_known_custom_object_id*");
    }

    [Fact(Skip = "Setup your own CustomObject and provide an ID for it.")]
    public void List_GivenExistingObject_ShouldGetResults()
    {
        // Arrange a custom object at https://app.hubspot.com/ with a property
        const string customProperty = "machine_name";
        const string idForCustomObject = "2-29369202";

        var opts = new ListRequestOptions
        {
            Limit = 2,
            PropertiesToInclude = new List<string> { "hs_created_by_user_id", customProperty }
        };

        var customObjectList = CustomObjectApi.List<CustomObjectHubSpotModel>(idForCustomObject, opts).Results;

        customObjectList.Should().NotBeNull();
    }
}