using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RetailBanking.Models;
using System.Net.Mail;

namespace LoanStatusNotification;

public class LoanNotificationFunction
{
    private readonly ILogger<LoanNotificationFunction> _logger;
    private readonly IConfiguration _config;
    public LoanNotificationFunction(ILogger<LoanNotificationFunction> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [Function(nameof(LoanNotificationFunction))]
    public async Task Run([ServiceBusTrigger("loannotificationqueue", Connection = "SbConnectionString")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        string connectionString =_config["ACSConnection"]!;
        string senderEmail =_config["SenderEmail"]!;
        var emailClient = new EmailClient(connectionString);

        var notification = message.Body.ToObjectFromJson<LoanMailResponse>();

        string subject = $" {notification!.CustomerName} -loan application status";

        string html = $@"<html><body>
            <h2>hello {notification.CustomerName},</h2>
            <p>your loan application status has been updated.</p>
            <p>
                <strong>loan id:</strong> {notification.LoanAppicationId}
            </p>
            <p>
                <strong>Loan Amount:</strong> {notification.LoanAmount}
            </p>
             <p>
                <strong>Interest Rate:</strong> {notification.InterestRate}
            </p>
            <p>
                <strong>status and Remarks:</strong> {notification.LoanStatus} - {notification.ErrorMessage}
            </p>
            <br/>
            <p>thank you for banking with us.</p>
            <p>
                regards,<br/>
                retail banking team
            </p>
        </body>
        </html>";

        var emailmessage = new EmailMessage(senderEmail,new EmailRecipients(new List<EmailAddress>
                {
                    new EmailAddress(notification.EmailAddress)
                }),
            new EmailContent(subject)
            {
                Html = html
            });

        _logger.LogInformation("Starting email send");

        try
        {
            var result = await emailClient.SendAsync(
                WaitUntil.Completed,
                emailmessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Email sending failed");
        }

        //await emailClient.SendAsync(WaitUntil.Completed, emailmessage);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}