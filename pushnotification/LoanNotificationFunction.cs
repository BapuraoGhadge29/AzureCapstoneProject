//using System.Text.Json;
//using Aspose.Email.Clients;
//using Azure;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

//public class SBLoanNotificationFunction
//{
//    private readonly IConfiguration _config;
//    private readonly ILogger<SBLoanNotificationFunction> _logger;

//    public SBLoanNotificationFunction(IConfiguration config,ILogger<SBLoanNotificationFunction> logger)
//    {
//        _config = config;
//        _logger = logger;
//    }

//    [Function("SBLoanNotificationFunction")]
//    public async Task Run([ServiceBusTrigger(_config["QueueName"]!, _config["SbConnectionString"]!)] string message)
//    {
//        try
//        {
//            var notification = JsonSerializer.Deserialize<LoanNotificationMessage>(message);

//            if (notification == null)
//            {
//                _logger.LogError("Invalid notification message received.");
//                return;
//            }

//            await SendEmail(notification);

//            _logger.LogInformation(
//                $"Email sent successfully for LoanId: {notification.LoanId}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error processing Service Bus message");
//            throw;
//        }
//    }
//    private async Task SendEmail(LoanNotificationMessage notification)
//    {
//        string connectionString =_config["ACSConnection"]!;

//        string senderEmail =_config["SenderEmail"]!;

//        var emailClient = new EmailClient(connectionString);

//        string subject = $"Loan Application Status - {notification.Status}";

//        string html = $@"<html><body>
//            <h2>Hello {notification.CustomerName},</h2>
//            <p>Your loan application status has been updated.</p>
//            <p>
//                <strong>Loan ID:</strong> {notification.LoanId}
//            </p>
//            <p>
//                <strong>Status:</strong> {notification.Status}
//            </p>
//            <br/>
//            <p>Thank you for banking with us.</p>
//            <p>
//                Regards,<br/>
//                Retail Banking Team
//            </p>
//        </body>
//        </html>";

//        var emailMessage = new EmailMessage(
//            senderEmail,
//            new EmailRecipients(
//                new List<EmailAddress>
//                {
//                    new EmailAddress(notification.Email)
//                }),
//            new EmailContent(subject)
//            {
//                Html = html
//            });

//        await emailClient.SendAsync(
//            WaitUntil.Completed,
//            emailMessage);
//    }
//}