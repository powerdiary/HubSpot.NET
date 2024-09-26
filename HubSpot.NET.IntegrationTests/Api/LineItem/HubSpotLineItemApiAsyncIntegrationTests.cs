using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.LineItem;
using HubSpot.NET.Api.LineItem.DTO;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.LineItem;

public sealed class HubSpotLineItemApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
{
    [Fact]
    public async Task CreateLineItem()
    {
        var (newLineItem, createdLineItem) = await PrepareLineItem();
        using (new AssertionScope())
        {
            createdLineItem.Id.Should().NotBeNull();
            createdLineItem.Properties.Name.Should().Be(newLineItem.Properties.Name);
            (await LineItemApi.GetByIdAsync<LineItemHubSpotModel>(createdLineItem.Id.Value)).Should().NotBeNull();
        }

        await LineItemApi.DeleteAsync(createdLineItem.Id ?? 0);
    }

    [Fact]
    public async Task CreateLineItem_WithAssociationToDeal_ShouldBeSuccessful()
    {
        var createdDeal = await CreateTestDeal("Test Deal for Line Item");

        var newLineItem = new LineItemHubSpotModel
        {
            Properties = new LineItemPropertiesHubSpotModel { Name = Guid.NewGuid().ToString() },
            Associations = { AssociatedDeals = new[] { createdDeal.Id.Value } }
        };
        var createdLineItem = await LineItemApi.CreateAsync(newLineItem);

        await Task.Delay(10000);  // Allow some time for the association to be registered

        var retrievedLineItem = await LineItemApi.GetByIdAsync<LineItemHubSpotModel>(createdLineItem.Id.Value);

        using (new AssertionScope())
        {
            createdLineItem.Associations.AssociatedDeals[0].Should().Be(createdDeal.Id.Value);
            retrievedLineItem.Associations.AssociatedDeals[0].Should().Be(createdDeal.Id.Value);
        }

        await LineItemApi.DeleteAsync(createdLineItem.Id ?? 0);
    }

    [Fact]
    public async Task GetLineItemById()
    {
        var (_, createdLineItem) = await PrepareLineItem();

        (await LineItemApi.GetByIdAsync<LineItemHubSpotModel>(createdLineItem.Id.Value)).Should().NotBe(0);

        await LineItemApi.DeleteAsync(createdLineItem.Id ?? 0);
    }

    [Fact]
    public async Task DeleteLineItem()
    {
        var (_, createdLineItem) = await PrepareLineItem();
        await LineItemApi.DeleteAsync(createdLineItem.Id ?? 0);
        await Task.Delay(5000);

        var act = async () => { await LineItemApi.GetByIdAsync<LineItemHubSpotModel>(createdLineItem.Id.Value); };

        await act.Should().ThrowAsync<HubSpotException>()
            .WithMessage("Line item with ID * does not exist*");
    }

    [Fact]
    public async Task ListLineItems()
    {
        var createdLineItems = new List<LineItemHubSpotModel>();

        var uniqueName1 = Guid.NewGuid().ToString("N");
        var uniqueName2 = Guid.NewGuid().ToString("N");

        var firstCreatedLineItem = await CreateTestLineItem($"Test Line Item {uniqueName1}");
        createdLineItems.Add(firstCreatedLineItem);

        var secondCreatedLineItem = await CreateTestLineItem($"Test Line Item {uniqueName2}");
        createdLineItems.Add(secondCreatedLineItem);

        var requestOptions = new LineItemListRequestOptions
        {
            PropertiesToInclude = new List<string> { "name" },
            Limit = 1
        };

        await Task.Delay(5000);

        var allLineItems = await LineItemApi.ListAsync<LineItemHubSpotModel>(requestOptions);

        using (new AssertionScope())
        {
            allLineItems.Should().NotBeNull();
            allLineItems.LineItems.Count.Should().Be(1);
            allLineItems.Paging.Next.After.Should().NotBeEmpty();
        }

        foreach (var lineItem in createdLineItems)
        {
            await LineItemApi.DeleteAsync(lineItem.Id.Value);
        }
    }

    [Fact]
    public async Task UpdateLineItem()
    {
        var createdLineItem = await CreateTestLineItem();
        createdLineItem.Properties.Name += " Updated";
        createdLineItem.Properties.Price += 100;

        var updatedLineItem = await LineItemApi.UpdateAsync(createdLineItem);
        ValidateLineItem(updatedLineItem);

        var retrievedLineItem = await LineItemApi.GetByIdAsync<LineItemHubSpotModel>(createdLineItem.Id.Value);
        ValidateLineItem(retrievedLineItem);

        await LineItemApi.DeleteAsync(createdLineItem.Id.Value);

        void ValidateLineItem(LineItemHubSpotModel lineItemToValidate)
        {
            using (new AssertionScope())
            {
                lineItemToValidate.Should().NotBeNull();
                lineItemToValidate.Id.Should().Be(createdLineItem.Id);
                lineItemToValidate.Properties.Name.Should().Be(createdLineItem.Properties.Name);
                lineItemToValidate.Properties.Price.Should().Be(createdLineItem.Properties.Price);
            }
        }
    }

    private async Task<(LineItemHubSpotModel NewLineItem, LineItemHubSpotModel CreatedLineItem)> PrepareLineItem()
    {
        var newLineItem = new LineItemHubSpotModel();

        newLineItem.Properties.Name = "Test Line Item " + Guid.NewGuid();
        newLineItem.Properties.Price = 100;

        var createdLineItem = await LineItemApi.CreateAsync(newLineItem);
        return (newLineItem, createdLineItem);
    }

    private Task<LineItemHubSpotModel> CreateTestLineItem(string? lineItemName = null)
    {
        lineItemName ??= "Test Line Item " + Guid.NewGuid();
        var newLineItem = new LineItemHubSpotModel
        {
            Properties = new LineItemPropertiesHubSpotModel
            {
                Name = lineItemName,
                Price = 100
            }
        };
        return LineItemApi.CreateAsync(newLineItem);
    }
}
