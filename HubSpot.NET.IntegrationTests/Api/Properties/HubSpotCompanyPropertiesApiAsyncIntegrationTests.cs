using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.Properties;

public class HubSpotCompanyPropertiesApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact(Skip = "This test requires CreatePropertyGroup implementation")]
    public async Task Create()
    {
        const string expectedName = "TestProperty";
        const string expectedType = "string";
        const string expectedFieldType = "text";
        const string expectedGroup = "TestGroup";
        const string expectedLabel = "TestLabel";

        var createdCompanyProperty =
            await RecreateTestCompanyPropertyAsync(name: expectedName, type: expectedType,
                fieldType: expectedFieldType,
                groupName: expectedGroup, label: expectedLabel);

        using (new AssertionScope())
        {
            createdCompanyProperty.Should().NotBeNull();
            createdCompanyProperty.Name.Should().Be(expectedName);
            createdCompanyProperty.Type.Should().Be(expectedType);
            createdCompanyProperty.FieldType.Should().Be(expectedFieldType);
        }
    }

    [Fact(Skip = "This test requires CreatePropertyGroup implementation")]
    public async Task GetAll()
    {
        var createdCompanyProperty1 =
            await RecreateTestCompanyPropertyAsync(name: "TestProperty1", type: "string", fieldType: "text");
        var createdCompanyProperty2 =
            await RecreateTestCompanyPropertyAsync(name: "TestProperty2", type: "string", fieldType: "text");

        var allProperties = CompanyPropertiesApi.GetAll().Results;

        using (new AssertionScope())
        {
            allProperties.Should().NotBeNull();
            allProperties.Count.Should()
                .BeGreaterOrEqualTo(2);
            allProperties.Should().Contain(p => p.Name == createdCompanyProperty1.Name);
            allProperties.Should().Contain(p => p.Name == createdCompanyProperty2.Name);
        }
    }

    [Fact]
    public async Task GetAll_ShouldNotThrowException()
    {
        var act = async () => (await CompanyPropertiesApi.GetAllAsync()).Results;
        await act.Should().NotThrowAsync<HubSpotException>();
    }
}