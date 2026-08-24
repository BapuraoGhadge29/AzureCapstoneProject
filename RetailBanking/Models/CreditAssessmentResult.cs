namespace RetailBanking.Models
{
    public class CreditAssessmentResult
    {
        public bool IsEligible { get; set; }

        public int RiskScore { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;
    }

}
