using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api;
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

        var firstCreatedDeal = CreateTestDeal($"Test Deal {uniqueName1}");
        createdDeals.Add(firstCreatedDeal);

        var secondCreatedDeal = CreateTestDeal($"Test Deal {uniqueName2}");
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

    [Fact]
    public void UpdateDeal()
    {
        var createdDeal = CreateTestDeal();
        createdDeal.Name += "New Name";
        createdDeal.Amount += 1;

        var updatedDeal = DealApi.Update(createdDeal);
        ValidateDeal(updatedDeal);

        var retrievedDeal = DealApi.GetById<DealHubSpotModel>(createdDeal.Id.Value);
        ValidateDeal(retrievedDeal);

        DealApi.Delete(createdDeal.Id.Value);

        void ValidateDeal(DealHubSpotModel dealToValidate)
        {
            using (new AssertionScope())
            {
                dealToValidate.Should().NotBeNull();
                dealToValidate.Id.Should().Be(createdDeal.Id);
                dealToValidate.Name.Should().Be(createdDeal.Name);
                dealToValidate.Amount.Should().Be(createdDeal.Amount);
            }
        }
    }
    
    [Fact]
    public async Task SearchDeals()
    {
        const double amount = 42;
        var createdDeal = CreateTestDeal(amount: amount);

        await Task.Delay(10000);
    
        var filterGroup = new SearchRequestFilterGroup { Filters = new List<SearchRequestFilter>() };
        filterGroup.Filters.Add(new SearchRequestFilter
        {
            PropertyName = "dealname",
            Operator = SearchRequestFilterOperatorType.EqualTo,
            Value = createdDeal.Name
        });
    
        var searchOptions = new SearchRequestOptions
        {
            FilterGroups = new List<SearchRequestFilterGroup>(),
            PropertiesToInclude = new List<string>
            {
                "dealname", "DateCreated", "dealstage", "amount",
                "closedate", "owner", "associatedcompanyid", "associatedcontactids", "dealtype"
            }
        };
    
        searchOptions.FilterGroups.Add(filterGroup);
    
        var searchResults = DealApi.Search<DealHubSpotModel>(searchOptions);
    
        using (new AssertionScope())
        {
            var foundDeal = searchResults.Results.FirstOrDefault(c => c.Id == createdDeal.Id);

            foundDeal.Should().NotBeNull("Expected to find the created deal in search results.");
            foundDeal.DateCreated.Should().NotBeNull("Expected CreatedAt to have a value.");
            foundDeal.Stage.Should().BeNull();
            foundDeal.OwnerId.Should().BeNull();
            foundDeal.DealType.Should().BeNull();
            foundDeal.Amount.Should().Be(amount);
            foundDeal.Name.Should().Be(createdDeal.Name);
        }

        DealApi.Delete(createdDeal.Id.Value);
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

    private DealHubSpotModel CreateTestDeal(string? dealName = null, double? amount = null)
    {
        dealName ??= "Unique Deal Name " + Guid.NewGuid().ToString("N");
        var newDeal = new DealHubSpotModel { Name = dealName, Amount = amount};
        return DealApi.Create(newDeal);
    }
}