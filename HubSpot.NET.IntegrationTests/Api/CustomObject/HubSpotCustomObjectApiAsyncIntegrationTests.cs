using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.CustomObject;

public sealed class HubSpotCustomObjectApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    private const string CustomObjectTypeName = "machine";

    [Fact]
    public async Task List_GivenUnknownObjectId_ShouldThrowException()
    {
        const string idForCustomObject = "unknown_known_custom_object_id";
        var opts = new ListRequestOptions();

        var act = async () => await CustomObjectApi.ListAsync<CustomObjectHubSpotModel>(idForCustomObject, opts);
        await act.Should().ThrowAsync<HubSpotException>()
            .WithMessage("*Unable to infer object type from: unknown_known_custom_object_id*");
    }

    [Fact]
    public async Task CreateWithDefaultAssociationToObjectAsync_ShouldCreateMachineObject()
    {
        const string associateObjectTypeName = "company";
        const string model = "XYZ MODEL";
        const string customProperty = "model";
        var company = await RecreateTestCompanyAsync();

        var machine = new CreateCustomObjectHubSpotModel
        {
            SchemaId = CustomObjectTypeName,
            Properties = new Dictionary<string, object>
            {
                { customProperty, model },
                { "year", "2022-01-01" },
                { "km", "5000" }
            }
        };

        var createdObjectId = await CustomObjectApi.CreateWithDefaultAssociationToObjectAsync<CreateCustomObjectHubSpotModel>(machine, associateObjectTypeName,
            company.Id.ToString());

        var opts = new ListRequestOptions
        {
            Limit = 4,
            PropertiesToInclude = new List<string> { "hs_created_by_user_id", customProperty }
        };

        var customObjectList = (await CustomObjectApi.ListAsync<CustomObjectHubSpotModel>(CustomObjectTypeName, opts)).Results;

        using (new AssertionScope())
        {
            customObjectList.Should().NotBeNull();
            customObjectList.Should().HaveCountGreaterThan(0);
            customObjectList.Should().OnlyContain(obj => obj.Properties.ContainsKey(customProperty));
            customObjectList.Any(x=> x.Properties[customProperty] == model && x.Id == createdObjectId).Should().BeTrue();
        }
    }

    [Fact]
    public async Task List_GivenExistingObject_ShouldGetResults()
    {
        const string customProperty = "model";

        await CreateCustomObjectMachine();

        var opts = new ListRequestOptions
        {
            Limit = 2,
            PropertiesToInclude = new List<string> { "hs_created_by_user_id", customProperty }
        };

        var customObjectList = (await CustomObjectApi.ListAsync<CustomObjectHubSpotModel>(CustomObjectTypeName, opts));

        using (new AssertionScope())
        {
            customObjectList.Results.Should().NotBeNull();
            customObjectList.Results.Should().HaveCountGreaterThan(0);
            customObjectList.Results.Should().OnlyContain(obj => obj.Properties.ContainsKey(customProperty));
        }
    }

    private async Task CreateCustomObjectMachine()
    {
        const string associateObjectTypeName = "company";
        var company = await RecreateTestCompanyAsync();

        var machine = new CreateCustomObjectHubSpotModel
        {
            SchemaId = CustomObjectTypeName,
            Properties = new Dictionary<string, object>
            {
                { "model", "Model X" },
                { "year", "2022-01-01" },
                { "km", "5000" }
            }
        };

        await CustomObjectApi.CreateWithDefaultAssociationToObjectAsync<CreateCustomObjectHubSpotModel>(machine, associateObjectTypeName,
            company.Id.ToString());
    }
}