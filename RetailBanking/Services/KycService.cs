using RetailBanking.Constants;
using RetailBanking.Interfaces;
using RetailBanking.Repository;

namespace RetailBanking.Services
{
    public class KycService : IKycService
    {
        private readonly RetailBankingDbContext _context;

        public KycService(RetailBankingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApproveKycAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return false;
            }

            customer.KycStatus = KycStatus.Approved.ToString();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectKycAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return false;
            }

            customer.KycStatus = KycStatus.Rejected.ToString();

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
