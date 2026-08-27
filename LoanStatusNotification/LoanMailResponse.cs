namespace RetailBanking.Models;
public class LoanMailResponse
{
    public int LoanAppicationId { get; set; }
    public string ErrorMessage { get; set; } = null!;
    public string LoanStatus { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
}

