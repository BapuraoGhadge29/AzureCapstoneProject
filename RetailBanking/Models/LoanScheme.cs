using RetailBanking.Constants;
using System.ComponentModel.DataAnnotations;

namespace RetailBanking.Models
{
    public class LoanScheme
    {
        [Key]
        public int SchemeId { get; set; }
        public string SchemeName { get; set; } 
        public LoanType LoanType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int MinTenureMonths { get; set; }
        public int MaxTenureMonths { get; set; }
        public string? Description { get; set; }
    }
}


