using System.ComponentModel.DataAnnotations;

namespace RetailBanking.Models
{
    public class KYCStatus
    {
        [Key]
        public int KYCStatusId { get; set; }
        public int CustomerId { get; set; }
        public DateTime VerifiedDate { get; set; }
        public string? RejectionReason { get; set; }
        public string? Status { get; set; }
    }
}
