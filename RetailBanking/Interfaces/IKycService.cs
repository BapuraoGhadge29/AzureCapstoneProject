namespace RetailBanking.Interfaces
{   
    public interface IKycService
    {
        Task<bool> ApproveKycAsync(int customerId);
        Task<bool> RejectKycAsync(int customerId);
    }
}
