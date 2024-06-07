using HubSpot.NET.Api.Associations;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.ContactList;
using HubSpot.NET.Api.CustomObject;
using HubSpot.NET.Api.Deal;
using HubSpot.NET.Api.Properties;
using HubSpot.NET.Core;
using Microsoft.Extensions.Configuration;

namespace HubSpot.NET.IntegrationTests;

public abstract class HubSpotIntegrationTestSetup : IDisposable
{
    protected readonly HubSpotAssociationsApi AssociationsApi;
    protected readonly HubSpotCompanyApi CompanyApi;
    protected readonly HubSpotContactApi ContactApi;
    protected readonly HubSpotContactListApi ContactListApi;
    protected readonly HubSpotCompaniesPropertiesApi CompanyPropertiesApi;
    protected readonly HubSpotCustomObjectApi CustomObjectApi;
    protected readonly HubSpotDealApi DealApi;


    protected readonly HubSpotApi HubSpotApi;

    protected readonly IList<long> CompaniesToCleanup = new List<long>();
    protected readonly IList<long> ContactsToCleanup = new List<long>();
    protected readonly IList<long> ContactListToCleanup = new List<long>();
    protected readonly IList<string> CompanyPropertiesToCleanup = new List<string>();

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
        HubSpotApi = new HubSpotApi(apiKey);
    }

    #region IDisposable Support
    private bool _isDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed || !disposing)
            return;

        CleanCompanies();
        CleanContacts();
        CleanContactLists();
        CleanCompanyProperties();

        _isDisposed = true;
    }

    private void CleanCompanies()
    {
        foreach (var companyId in CompaniesToCleanup)
        {
            CompanyApi.Delete(companyId);
        }
    }

    private void CleanContacts()
    {
        foreach (var contactId in ContactsToCleanup)
        {
            TryAction(() => ContactApi.Delete(contactId));
        }
    }

    private void CleanContactLists()
    {
        foreach (var contactListId in ContactListToCleanup)
        {
            TryAction(() => ContactListApi.DeleteContactList(contactListId));
        }
    }

    private void CleanCompanyProperties()
    {
        foreach (var propertyName in CompanyPropertiesToCleanup)
        {
            try
            {
                CompanyPropertiesApi.Delete(propertyName);
            }
            catch
            {
                // ignored
            }
        }
    }

    private void TryAction(Action action)
    {
        try
        {
            action();
        }
        catch
        {
            // ignored
        }
    }

    ~HubSpotIntegrationTestSetup()
    {
        Dispose(false);
    }
    #endregion
}