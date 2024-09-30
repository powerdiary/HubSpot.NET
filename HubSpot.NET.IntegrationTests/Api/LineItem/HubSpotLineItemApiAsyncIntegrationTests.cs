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
            createdLineItem.Properties.Price.Should().Be(newLineItem.Properties.Price);
            (await LineItemApi.GetByIdAsync<LineItemGetResponse>(createdLineItem.Id.Value)).Should().NotBeNull();
        }
    }

    [Fact]
    public async Task CreateLineItem_WithAssociationToDeal_ShouldBeSuccessful()
    {
        var createdDeal = await CreateTestDeal("Test Deal for Line Item");

        var newLineItem = new LineItemCreateOrUpdateRequest
        {
            Properties = new LineItemPropertiesHubSpotModel { Name = Guid.NewGuid().ToString() },
            AssociatedDeals = new[] { createdDeal.Id.Value }
        };
        var createdLineItem = await LineItemApi.CreateAsync<LineItemCreateOrUpdateRequest, LineItemGetResponse>(newLineItem);

        await Task.Delay(10000);  // Allow some time for the association to be registered

        var retrievedLineItem = await LineItemApi.GetByIdAsync<LineItemGetResponse>(createdLineItem.Id.Value);

        retrievedLineItem.Associations.Deals.Results[0].Id.Should().Be(createdDeal.Id.Value);
    }

    [Fact]
    public async Task GetLineItemById()
    {
        var (_, createdLineItem) = await PrepareLineItem();

        (await LineItemApi.GetByIdAsync<LineItemGetResponse>(createdLineItem.Id.Value)).Should().NotBe(0);
    }

    [Fact]
    public async Task DeleteLineItem()
    {
        var (_, createdLineItem) = await PrepareLineItem();
        await LineItemApi.DeleteAsync(createdLineItem.Id ?? 0);
        await Task.Delay(5000);

        var act = async () => { await LineItemApi.GetByIdAsync<LineItemGetResponse>(createdLineItem.Id.Value); };

        await act.Should().ThrowAsync<HubSpotException>()
            .WithMessage("Line item with ID * does not exist*");
    }

    [Fact]
    public async Task ListLineItems()
    {
        var createdLineItems = new List<LineItemGetResponse>();

        var uniqueName1 = Guid.NewGuid().ToString("N");
        var uniqueName2 = Guid.NewGuid().ToString("N");

        var firstCreatedLineItem = await CreateTestLineItem($"Test Line Item {uniqueName1}");
        createdLineItems.Add(firstCreatedLineItem.Item2);

        var secondCreatedLineItem = await CreateTestLineItem($"Test Line Item {uniqueName2}");
        createdLineItems.Add(secondCreatedLineItem.Item2);

        var requestOptions = new LineItemListRequestOptions
        {
            PropertiesToInclude = new List<string> { "name" },
            Limit = 1
        };

        await Task.Delay(5000);

        var allLineItems = await LineItemApi.ListAsync<LineItemGetResponse>(requestOptions);

        using (new AssertionScope())
        {
            allLineItems.Should().NotBeNull();
            allLineItems.LineItems.Count.Should().Be(1);
            allLineItems.Paging.Next.After.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task UpdateLineItem()
    {
        var (createLineItem, responseLineItem) = await CreateTestLineItem();
        createLineItem.Properties.Name += " Updated";
        createLineItem.Properties.Price += 100;
        createLineItem.Id = responseLineItem.Id;

        var updatedLineItem = await LineItemApi.UpdateAsync<LineItemCreateOrUpdateRequest, LineItemGetResponse>(createLineItem);

        responseLineItem.Properties.Name = createLineItem.Properties.Name;
        responseLineItem.Properties.Price = createLineItem.Properties.Price;
        ValidateLineItem(updatedLineItem);

        var retrievedLineItem = await LineItemApi.GetByIdAsync<LineItemGetResponse>(responseLineItem.Id.Value);
        ValidateLineItem(retrievedLineItem);

        void ValidateLineItem(LineItemGetResponse lineItemToValidate)
        {
            using (new AssertionScope())
            {
                lineItemToValidate.Should().NotBeNull();
                lineItemToValidate.Id.Should().Be(responseLineItem.Id);
                lineItemToValidate.Properties.Name.Should().Be(responseLineItem.Properties.Name);
                lineItemToValidate.Properties.Price.Should().Be(responseLineItem.Properties.Price);
            }
        }
    }

    private async Task<(LineItemCreateOrUpdateRequest NewLineItem, LineItemGetResponse CreatedLineItem)> PrepareLineItem()
    {
        var newLineItem = new LineItemCreateOrUpdateRequest();

        newLineItem.Properties.Name = "Test Line Item " + Guid.NewGuid();
        newLineItem.Properties.Price = 100;

        var createdLineItem = await LineItemApi.CreateAsync<LineItemCreateOrUpdateRequest, LineItemGetResponse>(newLineItem);
        return (newLineItem, createdLineItem);
    }
}
