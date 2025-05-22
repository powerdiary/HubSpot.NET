using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Api.CustomEvent.Dto;
using HubSpot.NET.Api.Schemas;
using HubSpot.NET.Core;

namespace HubSpot.NET.IntegrationTests.Api.CustomEvent
{
    public class HubSpotCustomEventApiAsyncIntegrationTests : HubSpotAsyncIntegrationTestBase
    {
        private const string EventName = "test_event1";

        [Fact]
        public async Task CreateEventDefinitionAsync_WhenValidEvent_ShouldCreateEvent()
        {
            // Arrange
            var eventDefinition = new EventDefinition
            {
                Name = "test_event_" + Guid.NewGuid().ToString("N"),
                Label = "Test Event",
                Labels = new SchemasLabelsModel { Singular = "Test Event" },
                Description = "Test event description",
                PrimaryObjectId = "0-1", // 0-1 is the ID for CONTACT
                TrackingType = "MANUAL"
            };

            // Act
            var result = await CustomEventApi.CreateEventDefinitionAsync(eventDefinition);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Name.Should().Be(eventDefinition.Name);
                result.Labels.Singular.Should().Be(eventDefinition.Labels.Singular);
                result.Description.Should().Be(eventDefinition.Description);
                result.PrimaryObjectId.Should().Be(eventDefinition.PrimaryObjectId);
                result.TrackingType.Should().Be(eventDefinition.TrackingType);
                result.Archived.Should().BeFalse();
            }
        }

        [Fact]
        public async Task SendEventTrackingDataForContact_WhenValidData_ShouldSucceedWithNoException()
        {            
            var eventDefinition = await GetTestEventDefinition();
            var contact = await RecreateTestContactAsync();

            var eventTracking = CreateTestEventTracking(contact.Email, eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().NotThrowAsync();            
        }

        [Fact]
        public async Task SendEventTrackingDataForContact_WhenInvalidEvent_ShouldThrowException()
        {
            var contact = await RecreateTestContactAsync();
            var eventTracking = CreateTestEventTracking(contact.Email, "nonexisting_event");

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().ThrowAsync<HubSpotException>();
        }

        [Fact]
        public async Task SendEventTrackingDataForContact_WhenInvalidEmail_ShouldNotThrowException()
        {
            var eventDefinition = await GetTestEventDefinition();

            var eventTracking = CreateTestEventTracking("invalid_email", eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().NotThrowAsync();
        }        

        [Fact]
        public async Task GetByNameAsync_WhenValidEventName_ShouldReturnEvent()
        {
            var result = await CustomEventApi.GetByNameAsync<EventDefinition>(EventName);

            result.Should().BeEquivalentTo(new EventDefinition
            {
                Name = EventName,
                Labels = new SchemasLabelsModel() { Singular = "Test Event1" }
            }, options =>
            options
                .Excluding(e => e.Description)
                .Excluding(e => e.FullyQualifiedName)
                .Excluding(e => e.Id)
                .Excluding(e => e.CreatedAt)
                .Excluding(e => e.UpdatedAt)
                .Excluding(e => e.Archived)
                .Excluding(e => e.TrackingType)
                .Excluding(e => e.PrimaryObjectId));
        }

        [Fact]
        public async Task SendEventTrackingDataForCompany_WhenValidData_ShouldSucceedWithNoException()
        {
            var company = await RecreateTestCompanyAsync();
            var eventDefinition = await GetTestEventDefinition("test_event2", "COMPANY");
            var eventTracking = CreateTestEventTracking(company.Id.Value, eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task SendEventTrackingDataForCompany_WhenInvalidObjectId_ShouldNotThrowException()
        {
            long randomNonExistingCompanyId = 10000234;
            var eventDefinition = await GetTestEventDefinition("test_event2", "COMPANY");
            var eventTracking = CreateTestEventTracking(randomNonExistingCompanyId, eventDefinition.FullyQualifiedName);

            Func<Task> act = async () => await CustomEventApi.SendEventTrackingData(eventTracking);

            await act.Should().NotThrowAsync();
        }

        private EventTracking CreateTestEventTracking(string email, string eventName)
        {            
            return new EventTracking
            {
                EventName = eventName,
                OccurredAt = DateTime.UtcNow.ToString("o"),
                Email = email
            };
        }

        private EventTracking CreateTestEventTracking(long id, string eventName)
        {
            return new EventTracking
            {
                EventName = eventName,
                OccurredAt = DateTime.UtcNow.ToString("o"),
                ObjectId = id.ToString()
            };
        }
    }
}
