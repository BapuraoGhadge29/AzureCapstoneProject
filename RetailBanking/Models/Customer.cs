using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RetailBanking.Models
{
    public partial class Customer
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public DateTime Dob { get; set; }
        public string? PAN { get; set; }
        public string? AadharNumber { get; set; }
        public string? Mobile { get; set; }
        public string? EmailAddress { get; set; }
        public string? EmploymentDetails { get; set; }
        public string? Income { get; set; }
        [NotMapped]
        public IFormFile? PanCard { get; set; }
        [NotMapped]
        public IFormFile? AadharCard { get; set; }
        [NotMapped]
        public IFormFile? IncomeProof { get; set; }
        public string? KycStatus { get; set; }
    }
}