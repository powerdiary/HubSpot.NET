using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.CustomObject;

public sealed class HubSpotCustomObjectApiIntegrationTests : HubSpotIntegrationTestBase
{
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
        const string associateObjectTypeName = "company";
        var company = RecreateTestCompany();

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

        return CustomObjectApi.CreateWithDefaultAssociationToObject<CreateCustomObjectHubSpotModel>(machine, associateObjectTypeName,
            company.Id.ToString());
    }
}