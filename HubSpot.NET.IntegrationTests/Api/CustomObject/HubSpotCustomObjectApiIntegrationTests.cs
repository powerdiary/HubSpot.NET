using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.CustomObject;

// WARNING: This test requires creation of the 'Machine' custom object schema. See README.md for more details.
public sealed class HubSpotCustomObjectApiIntegrationTests : HubSpotIntegrationTestBase
{
    private const string CustomPropertyModel = "model";
    private const string CustomPropertyYear = "year";
    private const string CustomObjectTypeName = "machine";
    private const string MachineModelValue = "Model X";
    private const string MachineYearValue = "2022-01-01";

    [Fact]
    public void List_GivenUnknownObjectId_ShouldThrowException()
    {
        const string idForCustomObject = "unknown_known_custom_object_id";
        var opts = new ListRequestOptions();

        var act = () => CustomObjectApi.List<CustomObjectHubSpotModel>(idForCustomObject, opts).Results;
        act.Should().Throw<HubSpotException>()
            .WithMessage("*Unable to infer object type from: unknown_known_custom_object_id*");
    }

    [Fact]
    public void List_GivenExistingObject_ShouldGetResults()
    {
        CreateCustomObjectMachine();

        var opts = new ListRequestOptions
        {
            Limit = 10,
            PropertiesToInclude = new List<string> { CustomPropertyModel }
        };

        var customObjectList = CustomObjectApi.List<CustomObjectHubSpotModel>(CustomObjectTypeName, opts);

        using (new AssertionScope())
        {
            customObjectList.Results.Should().NotBeNull();
            customObjectList.Results.Should().HaveCountGreaterThan(0);
            customObjectList.Results.Should().OnlyContain(obj => obj.Properties.ContainsKey(CustomPropertyModel));
        }
    }

    [Fact]
    public void CreateCustomObject_ShouldCreateMachineObject()
    {
        var machine = new CreateCustomObjectHubSpotModel
        {
            SchemaId = CustomObjectTypeName,
            Properties = new Dictionary<string, object>
            {
                { CustomPropertyModel, MachineModelValue },
                { CustomPropertyYear, MachineYearValue },
                { "km", "5000" }
            }
        };

        var createdMachine = CustomObjectApi.CreateObject<CreateCustomObjectHubSpotModel, CustomObjectHubSpotModel>(machine);

        using (new AssertionScope())
        {
            createdMachine.Id.Should().NotBeEmpty();
            createdMachine.Properties.Should().ContainKey(CustomPropertyModel);
            createdMachine.Properties.Should().ContainKey(CustomPropertyYear);
            createdMachine.Properties.Should().Contain(new KeyValuePair<string, string>(CustomPropertyModel, MachineModelValue));
            createdMachine.Properties.Should().Contain(new KeyValuePair<string, string>(CustomPropertyYear, MachineYearValue));
        }

        CustomObjectApi.DeleteObject(CustomObjectTypeName, createdMachine.Id);
    }

    [Fact]
    public void GetCustomObject_ShouldGetMachineObject()
    {
        const string customPropertyModel = "model";
        const string customPropertyYear = "year";
        var propertiesToInclude = new List<string> { customPropertyModel, customPropertyYear };
        var machineId = CreateCustomObjectMachine();

        var customObject = CustomObjectApi.GetObject<CustomObjectHubSpotModel>(CustomObjectTypeName, machineId, propertiesToInclude);

        using (new AssertionScope())
        {
            customObject.Id.Should().Be(machineId);
            customObject.Properties.Should().ContainKey(customPropertyModel);
            customObject.Properties.Should().ContainKey(customPropertyYear);
            customObject.Properties.Should().Contain(new KeyValuePair<string, string>(customPropertyModel, MachineModelValue));
            customObject.Properties.Should().Contain(new KeyValuePair<string, string>(customPropertyYear, MachineYearValue));
        }

        CustomObjectApi.DeleteObject(CustomObjectTypeName, customObject.Id);
    }

    [Fact]
    public void UpdateCustomObject_ShouldUpdateMachineObject()
    {
        const string customPropertyModel = "model";
        const string customPropertyYear = "year";

        const string customPropertyModelValue = "Nissan Leaf";
        const string customPropertyYearValue = "2012-01-01";
        var propertiesToInclude = new List<string> { customPropertyModel, customPropertyYear };
        var machineId = CreateCustomObjectMachine();

        var customObject = CustomObjectApi.GetObject<CustomObjectHubSpotModel>(CustomObjectTypeName, machineId, propertiesToInclude);

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

        var updatedCustomObject = CustomObjectApi.UpdateObject<UpdateCustomObjectHubSpotModel, CustomObjectHubSpotModel>(updateCustomObjectHubSpotModel);

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

        CustomObjectApi.DeleteObject(CustomObjectTypeName, customObject.Id);
    }

    [Fact]
    public void DeleteCustomObject_ShouldDeleteMachine()
    {
        var machineId = CreateCustomObjectMachine();
        CustomObjectApi.DeleteObject(CustomObjectTypeName, machineId);

        var act = () => CustomObjectApi.GetObject<CustomObjectHubSpotModel>(CustomObjectTypeName, machineId, null);

        act.Should().Throw<HubSpotException>()
            .WithMessage("Error from HubSpot, Response = Status: NotFound; Description: Not Found");
    }

    private string CreateCustomObjectMachine()
    {
        var machine = new CreateCustomObjectHubSpotModel
        {
            SchemaId = CustomObjectTypeName,
            Properties = new Dictionary<string, object>
            {
                { CustomPropertyModel, MachineModelValue },
                { CustomPropertyYear, MachineYearValue },
                { "km", "5000" }
            }
        };

        return CustomObjectApi.CreateObject<CreateCustomObjectHubSpotModel, CustomObjectHubSpotModel>(machine).Id;
    }
}