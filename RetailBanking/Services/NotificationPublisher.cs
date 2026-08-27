using Azure.Messaging.ServiceBus;
using RetailBanking.Models;
using System.Text.Json;

namespace RetailBanking.Services
{
    public class NotificationPublisher
    {
        private readonly IConfiguration _configuration;

        public NotificationPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishAsync(LoanResponse notification)
        {
            string connectionString = _configuration["ServiceBus:SbConnectionString"]!;

            string queueName = _configuration["ServiceBus:QueueName"]!;

            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(queueName);

            string json = JsonSerializer.Serialize(notification);

            await sender.SendMessageAsync(new ServiceBusMessage(json));
        }
    }
}
