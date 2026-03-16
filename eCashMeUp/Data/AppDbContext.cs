using eCashMeUp.Models;
using Microsoft.EntityFrameworkCore;

namespace eCashMeUp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<EmploymentDetail> EmploymentDetails { get; set; }
        public DbSet<FinancialDetail> FinancialDetails { get; set; }
        public DbSet<BankingDetail> BankingDetails { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<RepaymentSchedule> RepaymentSchedules { get; set; }
        public DbSet<CreditAssessment> CreditAssessments { get; set; }
        public DbSet<LoanDisbursement> LoanDisbursements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User -> EmploymentDetail (1 to 1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.EmploymentDetail)
                .WithOne(e => e.User)
                .HasForeignKey<EmploymentDetail>(e => e.UserId);

            // User -> FinancialDetail (1 to 1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.FinancialDetail)
                .WithOne(f => f.User)
                .HasForeignKey<FinancialDetail>(f => f.UserId);

            // User -> BankingDetail (1 to 1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.BankingDetail)
                .WithOne(b => b.User)
                .HasForeignKey<BankingDetail>(b => b.UserId);

            // User -> LoanApplications (1 to many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.LoanApplications)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            // LoanApplication -> RepaymentSchedules (1 to many)
            modelBuilder.Entity<LoanApplication>()
                .HasMany(l => l.RepaymentSchedules)
                .WithOne(r => r.LoanApplication)
                .HasForeignKey(r => r.ApplicationId);

            modelBuilder.Entity<FinancialDetail>()
                .Property(f => f.NetDisposableIncome)
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}