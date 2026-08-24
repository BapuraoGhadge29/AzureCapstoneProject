using Microsoft.EntityFrameworkCore;
using RetailBanking.Models;

namespace RetailBanking.Repository;
public partial class RetailBankingDbContext : DbContext
{
    public RetailBankingDbContext()
    {
    }

    public RetailBankingDbContext(DbContextOptions<RetailBankingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LoanApplication> LoanApplications { get; set; }
    public virtual DbSet<LoanScheme> LoanSchemes { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<KYCStatus> KYCStatuses { get; set; }
}

