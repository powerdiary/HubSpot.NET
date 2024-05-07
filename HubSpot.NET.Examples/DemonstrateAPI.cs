using System;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;

namespace HubSpot.NET.Examples
{
    public class DemonstrateApi
    {
        private readonly HubSpotApi _api = null;

        public DemonstrateApi(string apiKey)
        {
            _api = new HubSpotApi(apiKey);
        }

        public void CreateAndDeleteEntities()
        {
            CompanyHubSpotModel createdCompany = null;
            ContactHubSpotModel createdContact = null;

            try
            {
                // Create a new contact
                ContactHubSpotModel newContact = new ContactHubSpotModel
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "testuser@test.com",
                    Company = "Test Company"
                };

                createdContact = _api.Contact.Create(newContact);
                Console.WriteLine($"Contact created with ID: {createdContact.Id}");

                // Create a new company
                CompanyHubSpotModel newCompany = new CompanyHubSpotModel
                {
                    Name = "New Company",
                    Description = "This is a new test company",
                };

                createdCompany = _api.Company.Create(newCompany);
                Console.WriteLine($"Company created with ID: {createdCompany.Id}");

                // Associate the contact with the company
                _api.Associations.AssociationToObject(
                    HubSpot.NET.Core.HubSpotObjectIds.Contact,
                    createdContact.Id.Value.ToString(),
                    HubSpot.NET.Core.HubSpotObjectIds.Company,
                    createdCompany.Id.Value.ToString());

                Console.WriteLine($"Contact {createdContact.Id} is now associated with company {createdCompany.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Clean up: Delete the contact and company
                if (createdContact != null)
                {
                    _api.Contact.Delete(createdContact.Id.Value);
                    Console.WriteLine($"Contact with ID {createdContact.Id} is deleted.");
                }

                if (createdCompany != null)
                {
                    _api.Company.Delete(createdCompany.Id.Value);
                    Console.WriteLine($"Company with ID {createdCompany.Id} is deleted.");
                }

                // The association is deleted once any part of it is deleted.
            }
        }
    }
}