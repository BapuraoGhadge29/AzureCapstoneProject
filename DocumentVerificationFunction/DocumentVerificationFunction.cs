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
    private async Task SendNotificationAsync(string customerId,string status)
    {
        var client = new HttpClient();
        string? emailAddress;

        using SqlConnection conn = new SqlConnection(_config["SqlConnectionString"]!);
        await conn.OpenAsync();        

        using (SqlCommand cmd = new SqlCommand("SELECT EmailAddress FROM Customers WHERE Id=@CustomerId", conn))
        {
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            emailAddress = Convert.ToString(await cmd.ExecuteScalarAsync());
        }

        var payload = new
        {
            customerId,
            emailAddress,
            status
        };

        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload),Encoding.UTF8,"application/json");

        string logicAppUrl = _config["LogicAppUrl"]!;

        await client.PostAsync(logicAppUrl, content);
    }
}
