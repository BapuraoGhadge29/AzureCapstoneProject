using RetailBanking.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RetailBanking.Models
{
    public class LoanApplication
    {
        [JsonIgnore]
        [Key]
        public int LoanApplicationId { get; set; }
        public LoanType LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public int TenureInMonths { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string EmploymentType { get; set; }
        [JsonIgnore]
        public string? ApplicationStatus { get; set; } 
        public DateTime ApplicationDate { get; set; }
        public int CustomerId { get; set; }
        public int CreditScore { get; set; }
        public decimal AnnualIncome { get; set; }
        public int EmploymentYears { get; set; }
    }
}
