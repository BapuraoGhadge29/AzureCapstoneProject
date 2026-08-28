using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DocumentVerificationFunction;

public class DocumentVerificationFunction
{
    private readonly ILogger<DocumentVerificationFunction> _logger;
    private readonly IConfiguration _config;

    public DocumentVerificationFunction(ILogger<DocumentVerificationFunction> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [Function(nameof(DocumentVerificationFunction))]
    public async Task Run([BlobTrigger("customerdocuments/{customerId}/{fileName}", Connection = "AzureWebJobsStorage")] Stream stream, string customerId, string name)
    {
        string storageConnection = _config["AzureWebJobsStorage"]!;

        BlobContainerClient container = new BlobContainerClient(storageConnection, "customerdocuments");

        bool panFound = false;
        bool aadharFound = false;

        await foreach (var item in container.GetBlobsAsync(prefix: $"{customerId}/"))
        {
            string Itemname = item.Name.ToUpper();

            if (Itemname.Contains("PAN"))
                panFound = true;

            if (Itemname.Contains("AADHAR") || Itemname.Contains("AADHAAR"))
                aadharFound = true;
        }

        string status = (panFound && aadharFound) ? "Approved": "Rejected";

        await UpdateCustomerStatus(Convert.ToInt32(customerId),status);

        await SendNotificationAsync(customerId,status);        
    }

    private async Task UpdateCustomerStatus(int customerId,string status)
    {     

        using SqlConnection conn = new SqlConnection(_config["SqlConnectionString"]!);
        await conn.OpenAsync();

        string query = @" UPDATE Customers SET KycStatus = @Status WHERE Id = @CustomerId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Status",status);
        cmd.Parameters.AddWithValue("@CustomerId",customerId);

        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }
    private async Task SendNotificationAsync(string customerId, string status)
    {
        string? emailAddress = null;
        string? fullName = null;
        bool isNotificationSent = false;

        using SqlConnection conn = new SqlConnection(_config["SqlConnectionString"]!);
        await conn.OpenAsync();

        string selectQuery = @"SELECT EmailAddress, FullName, IsNotificationSent FROM Customers WHERE Id = @CustomerId";

        using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
        {
            cmd.Parameters.AddWithValue("@CustomerId", customerId);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                emailAddress = reader["EmailAddress"]?.ToString();
                fullName = reader["FullName"]?.ToString();
                if (reader["IsNotificationSent"] != DBNull.Value)
                {
                    isNotificationSent = true;
                }
                else
                {
                    isNotificationSent = false;
                }
            }
        }

        // Skip if notification already sent
        if (isNotificationSent)
        {
            return;
        }

        var payload = new
        {
            fullName,
            customerId,
            emailAddress,
            status
        };

        using var client = new HttpClient();

        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        string logicAppUrl = _config["LogicAppUrl"]!;       

        string updateQuery = @"UPDATE Customers SET IsNotificationSent = 1 WHERE Id = @CustomerId";
        using SqlCommand cmd1 = new SqlCommand(updateQuery, conn);
        cmd1.Parameters.AddWithValue("@CustomerId", customerId);

        HttpResponseMessage response = await client.PostAsync(logicAppUrl, content);
        if(!response.IsSuccessStatusCode)
        {
            updateQuery = @"UPDATE Customers SET IsNotificationSent = o WHERE Id = @CustomerId";
            using SqlCommand cmd2 = new SqlCommand(updateQuery, conn);
            cmd1.Parameters.AddWithValue("@CustomerId", customerId);
        }

        await cmd1.ExecuteNonQueryAsync();

    }
}
