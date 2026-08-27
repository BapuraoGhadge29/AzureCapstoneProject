using System.ComponentModel.DataAnnotations;

namespace RetailBanking.Models
{
    public class DocumentDetails
    {
        [Key]
        public int DocumentId { get; set; }
        public int CustomerId { get; set; }        
        public string? DocumentPath { get; set; }
    }
}
