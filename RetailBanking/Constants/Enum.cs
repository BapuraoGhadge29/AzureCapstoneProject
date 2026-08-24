namespace RetailBanking.Constants
{
    public class Enum
    {
    }
    public enum LoanType
    {
        PersonalLoan = 1,
        HomeLoan = 2,
        VehicleLoan = 3,
        EducationLoan = 4
    }

    public enum ApplicationStatus
    {
        Submitted = 1,
        UnderReview = 2,
        DocumentsPending = 3,
        Approved = 4,
        Rejected = 5
    }
    public enum KycStatus
    {
        Pending = 1,
        Submitted = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5
    }
}


