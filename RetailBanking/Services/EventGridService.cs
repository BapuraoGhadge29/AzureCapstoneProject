using Azure;
using Azure.Messaging.EventGrid;

namespace RetailBanking.Services
{   
    public class EventGridService
    {
        private readonly EventGridPublisherClient _client;

        public EventGridService(IConfiguration configuration)
        {
            var endpoint = new Uri(configuration["EventGrid:TopicEndpoint"]!);
            var key = configuration["EventGrid:AccessKey"];
            _client = new EventGridPublisherClient(endpoint, new AzureKeyCredential(key!));
        }

        public async Task PublishCustomerCreated(int customerId,string customerName)
        {
            var eventGridEvent = new EventGridEvent(
                subject: $"Customer/{customerId}",
                eventType: "Customer.Created",
                dataVersion: "1.0",
                data: new
                {
                    CustomerId = customerId,
                    CustomerName = customerName
                });

            await _client.SendEventAsync(eventGridEvent);
        }
    }
}
