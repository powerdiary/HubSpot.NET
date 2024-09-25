using HubSpot.NET.Api.Associations;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.ContactList;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Api.Deal;
using HubSpot.NET.Api.LineItem.HubSpot.NET.Api.LineItems;
using HubSpot.NET.Api.Properties;
using HubSpot.NET.Core;
using Microsoft.Extensions.Configuration;

namespace HubSpot.NET.IntegrationTests;

using Xunit;

public abstract class HubSpotIntegrationTestSetup : IAsyncLifetime
{
    protected readonly HubSpotAssociationsApi AssociationsApi;
    protected readonly HubSpotCompanyApi CompanyApi;
    protected readonly HubSpotContactApi ContactApi;
    protected readonly HubSpotContactListApi ContactListApi;
    protected readonly HubSpotCompaniesPropertiesApi CompanyPropertiesApi;
    protected readonly HubSpotCustomObjectApi CustomObjectApi;
    protected readonly HubSpotDealApi DealApi;
    protected readonly HubSpotLineItemApi LineItemApi;

    protected readonly HubSpotApi HubSpotApi;

    protected readonly IList<long> CompaniesToCleanup = new List<long>();
    protected readonly IList<long> ContactsToCleanup = new List<long>();
    protected readonly IList<long> ContactListToCleanup = new List<long>();
    protected readonly IList<string> CompanyPropertiesToCleanup = new List<string>();
    protected readonly IList<long> DealsToCleanup = new List<long>();
    protected readonly IList<long> LineItemsToCleanup = new List<long>();

    protected HubSpotIntegrationTestSetup()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
                optional: true)
            .Build();

        var apiKey = configuration["HubSpot:PrivateAppKey"];

        var client = new HubSpotBaseClient(apiKey);
        AssociationsApi = new HubSpotAssociationsApi(client);
        CompanyApi = new HubSpotCompanyApi(client);
        ContactApi = new HubSpotContactApi(client);
        ContactListApi = new HubSpotContactListApi(client);
        CompanyPropertiesApi = new HubSpotCompaniesPropertiesApi(client);
        CustomObjectApi = new HubSpotCustomObjectApi(client, AssociationsApi);
        DealApi = new HubSpotDealApi(client);
        LineItemApi = new HubSpotLineItemApi(client);
        HubSpotApi = new HubSpotApi(apiKey);
    }

    public async Task InitializeAsync()
    {
        // Any async initialization can be placed here if necessary
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Perform async cleanup
        await CleanCompaniesAsync();
        await CleanContactsAsync();
        await CleanContactListsAsync();
        await CleanCompanyPropertiesAsync();
        await CleanDealsAsync();
        await CleanLineItemsAsync();
    }

    private async Task CleanCompaniesAsync()
    {
        foreach (var companyId in CompaniesToCleanup)
        {
            try
            {
                await CompanyApi.DeleteAsync(companyId);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }

    private async Task CleanContactsAsync()
    {
        foreach (var contactId in ContactsToCleanup)
        {
            try
            {
                await ContactApi.DeleteAsync(contactId);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }

    private async Task CleanContactListsAsync()
    {
        foreach (var contactListId in ContactListToCleanup)
        {
            try
            {
                await ContactListApi.DeleteContactListAsync(contactListId);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }

    private async Task CleanCompanyPropertiesAsync()
    {
        foreach (var propertyName in CompanyPropertiesToCleanup)
        {
            try
            {
                await CompanyPropertiesApi.DeleteAsync(propertyName);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }

    private async Task CleanDealsAsync()
    {
        foreach (var dealId in DealsToCleanup)
        {
            try
            {
                await DealApi.DeleteAsync(dealId);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }

    private async Task CleanLineItemsAsync()
    {
        var deleteTasks = LineItemsToCleanup.Select(lineItemId => LineItemApi.DeleteAsync(lineItemId));
        try
        {
            await Task.WhenAll(deleteTasks);
        }
        catch
        {
            // Ignore errors during cleanup
        }
    }
}

