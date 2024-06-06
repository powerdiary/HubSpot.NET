using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.Deal;
using HubSpot.NET.Api.Deal.Dto;

namespace HubSpot.NET.IntegrationTests.Api.Deal;

public sealed class HubSpotDealApiIntegrationTests : HubSpotIntegrationTestBase
{
    [Fact]
    public async Task CreateDeal()
    {
        var (newDeal, createdDeal) = await PrepareDeal();
        using (new AssertionScope())
        {
            createdDeal.Id.Should().NotBeNull();
            createdDeal.Name.Should().Be(newDeal.Name);
            DealApi.GetById<DealHubSpotModel>(createdDeal.Id.Value).Should().NotBeNull();
        }

        DealApi.Delete(createdDeal.Id ?? 0);
    }

    [Fact]
    public async Task GetDealById()
    {
        var (_, createdDeal) = await PrepareDeal();
        using (new AssertionScope())
        {
            DealApi.GetById<DealHubSpotModel>(createdDeal.Id.Value).Should().NotBeNull();
        }

        DealApi.Delete(createdDeal.Id ?? 0);
    }

    [Fact]
    public async Task DeleteDeal()
    {
        var (_, createdDeal) = await PrepareDeal();
        DealApi.Delete(createdDeal.Id ?? 0);
        await Task.Delay(5000);
        DealApi.GetById<DealHubSpotModel>(createdDeal.Id.Value).Should().BeNull();
    }

    [Fact]
    public async Task ListDeals()
    {
        var createdDeals = new List<DealHubSpotModel>();

        var uniqueName1 = Guid.NewGuid().ToString("N");
        var uniqueName2 = Guid.NewGuid().ToString("N");

        var firstCreatedDeal = RecreateTestDeal($"Test Deal {uniqueName1}");
        createdDeals.Add(firstCreatedDeal);

        var secondCreatedDeal = RecreateTestDeal($"Test Deal {uniqueName2}");
        createdDeals.Add(secondCreatedDeal);

        var requestOptions = new DealListRequestOptions
        {
            PropertiesToInclude = new List<string> { "dealname" },
            Limit = 1
        };

        await Task.Delay(5000);

        var allDeals = DealApi.List<DealHubSpotModel>(requestOptions);

        using (new AssertionScope())
        {
            allDeals.Should().NotBeNull();
            allDeals.Deals.Count.Should().Be(1);
            allDeals.Paging.Next.After.Should().NotBeEmpty();
            allDeals.Paging.Next.Link.Should().Contain("https://api.hubapi.com/crm/v3/objects/deals?");
        }

        foreach (var deal in createdDeals)
        {
            DealApi.Delete(deal.Id.Value);
        }
    }

    private async Task<(DealHubSpotModel NewDeal, DealHubSpotModel CreatedDeal)> PrepareDeal()
    {
        var newDeal = new DealHubSpotModel
        {
            Name = Guid.NewGuid().ToString()
        };
        var createdDeal = DealApi.Create(newDeal);
        await Task.Delay(5000);
        return (newDeal, createdDeal);
    }

    private DealHubSpotModel RecreateTestDeal(string dealName)
    {
        var newDeal = new DealHubSpotModel { Name = dealName };
        return DealApi.Create(newDeal);
    }
}