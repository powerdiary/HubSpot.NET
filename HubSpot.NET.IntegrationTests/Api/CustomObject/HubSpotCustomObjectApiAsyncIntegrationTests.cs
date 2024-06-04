using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.CustomObject;

public sealed class HubSpotCustomObjectApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    private const string CustomObjectTypeName = "machine";
    private const string MachineModelValue = "Model X";
    private const string MachineYearValue = "2022-01-01";

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

        var customObjectList = await CustomObjectApi.ListAsync<CustomObjectHubSpotModel>(CustomObjectTypeName, opts);

        using (new AssertionScope())
        {
            customObjectList.Results.Should().NotBeNull();
            customObjectList.Results.Should().HaveCountGreaterThan(0);
            customObjectList.Results.Should().OnlyContain(obj => obj.Properties.ContainsKey(customProperty));
        }
    }

    [Fact]
    public async Task GetCustomObjectAsync_ShouldGetMachineObject()
    {
        const string customPropertyModel = "model";
        const string customPropertyYear = "year";
        var propertiesToInclude = new List<string> { customPropertyModel, customPropertyYear };
        var machineId = await CreateCustomObjectMachine();

        var customObject = await CustomObjectApi.GetObjectAsync<CustomObjectHubSpotModel>(CustomObjectTypeName, machineId, propertiesToInclude);

        using (new AssertionScope())
        {
            customObject.Id.Should().Be(machineId);
            customObject.Properties.Should().ContainKey(customPropertyModel);
            customObject.Properties.Should().ContainKey(customPropertyYear);
            customObject.Properties.Should().Contain(new KeyValuePair<string, string>(customPropertyModel, MachineModelValue));
            customObject.Properties.Should().Contain(new KeyValuePair<string, string>(customPropertyYear, MachineYearValue));
        }

        await CustomObjectApi.DeleteObjectAsync(CustomObjectTypeName, customObject.Id);
    }

    [Fact]
    public async Task UpdateCustomObject_ShouldUpdateMachineObject()
    {
        const string customPropertyModel = "model";
        const string customPropertyYear = "year";

        const string customPropertyModelValue = "Nissan Leaf";
        const string customPropertyYearValue = "2012-01-01";
        var propertiesToInclude = new List<string> { customPropertyModel, customPropertyYear };
        var machineId = await CreateCustomObjectMachine();

        var customObject = await CustomObjectApi.GetObjectAsync<CustomObjectHubSpotModel>(CustomObjectTypeName, machineId, propertiesToInclude);

        var updateCustomObjectHubSpotModel = new UpdateCustomObjectHubSpotModel
        {
            Id = customObject.Id,
            SchemaId = CustomObjectTypeName,
            Properties = new Dictionary<string, object>
            {
                {customPropertyModel, customPropertyModelValue},
                {customPropertyYear, customPropertyYearValue}
            }
        };

        var updatedCustomObject = await CustomObjectApi.UpdateObjectAsync<UpdateCustomObjectHubSpotModel, CustomObjectHubSpotModel>(updateCustomObjectHubSpotModel);

        using (new AssertionScope())
        {
            updatedCustomObject.Id.Should().Be(customObject.Id);
            updatedCustomObject.Properties.Should().ContainKey(customPropertyModel);
            updatedCustomObject.Properties.Should().ContainKey(customPropertyYear);
            updatedCustomObject.Properties.Should().NotContain(new KeyValuePair<string, string>(customPropertyModel, MachineModelValue));
            updatedCustomObject.Properties.Should().NotContain(new KeyValuePair<string, string>(customPropertyYear, MachineYearValue));
            updatedCustomObject.Properties.Should().Contain(new KeyValuePair<string, string>(customPropertyModel, customPropertyModelValue));
            updatedCustomObject.Properties.Should().Contain(new KeyValuePair<string, string>(customPropertyYear, customPropertyYearValue));
        }

        await CustomObjectApi.DeleteObjectAsync(CustomObjectTypeName, customObject.Id);
    }

    [Fact]
    public async Task DeleteCustomObjectAsync_ShouldDeleteMachine()
    {
        var machineId = await CreateCustomObjectMachine();
        await CustomObjectApi.DeleteObjectAsync(CustomObjectTypeName, machineId);

        var act = async () => await CustomObjectApi.GetObjectAsync<CustomObjectHubSpotModel>(CustomObjectTypeName, machineId, null);

        (await act.Should().ThrowAsync<HubSpotException>())
            .WithMessage("Error from HubSpot, Response = Status: NotFound; Description: Not Found");
    }

    private async Task<string> CreateCustomObjectMachine()
    {
        const string associateObjectTypeName = "company";
        var company = await RecreateTestCompanyAsync();

        var machine = new CreateCustomObjectHubSpotModel
        {
            SchemaId = CustomObjectTypeName,
            Properties = new Dictionary<string, object>
            {
                { "model", MachineModelValue },
                { "year", MachineYearValue },
                { "km", "5000" }
            }
        };

        return await CustomObjectApi.CreateWithDefaultAssociationToObjectAsync<CreateCustomObjectHubSpotModel>(machine, associateObjectTypeName,
            company.Id.ToString());
    }
}