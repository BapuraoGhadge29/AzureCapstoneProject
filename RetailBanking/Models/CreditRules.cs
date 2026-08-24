namespace RetailBanking.Models
{
    public class CreditRules
    {
        public decimal MinIncome { get; set; }

        public int MinCreditScore { get; set; }

        public int AutoApproveRiskScore { get; set; }

        public int ManualReviewRiskScore { get; set; }
    }
}
