using FluentAssertions;
using FluentAssertions.Execution;

namespace HubSpot.NET.IntegrationTests.Api.Properties;

public class HubSpotCompanyPropertiesApiIntegrationTests : HubSpotIntegrationTestBase
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
            RecreateTestCompanyProperty(name: expectedName, type: expectedType, fieldType: expectedFieldType,
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
    public void GetAll()
    {
        var createdCompanyProperty1 =
            RecreateTestCompanyProperty(name: "TestProperty1", type: "string", fieldType: "text");
        var createdCompanyProperty2 =
            RecreateTestCompanyProperty(name: "TestProperty2", type: "string", fieldType: "text");

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
}