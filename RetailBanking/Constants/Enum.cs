using System.ComponentModel;

namespace RetailBanking.Constants
{
    public class Enum
    {
    }
    public enum LoanType
    {       
        [Description("PersonalLoan")] PersonalLoan,
        [Description("HomeLoan")] HomeLoan,
        [Description("VehicleLoan")] VehicleLoan,
        [Description("EducationLoan")] EducationLoan
    }

    public enum ApplicationStatus
    {
        [Description("Submitted")] Submitted,
        [Description("UnderReview")] UnderReview,
        [Description("Approved")] Approved,
        [Description("Rejected")] Rejected
    }
    public enum KycStatus
    {
        [Description("Pending")] Pending,
        [Description("Submitted")] Submitted,
        [Description("UnderReview")] UnderReview,
        [Description("Approved")] Approved,
        [Description("Rejected")] Rejected
    }
}


