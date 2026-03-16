using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("loan_applications")]
    public class LoanApplication
    {
        [Key]
        [Column("application_id")]
        public int ApplicationId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; } = 1;

        [Column("loan_amount")]
        public decimal LoanAmount { get; set; }

        [Column("loan_term_months")]
        public int LoanTermMonths { get; set; }

        [Column("interest_rate")]
        public decimal InterestRate { get; set; } = 15.00m;

        [Column("monthly_installment")]
        public decimal MonthlyInstallment { get; set; }

        [Column("total_repayable")]
        public decimal TotalRepayable { get; set; }

        [Column("total_interest")]
        public decimal TotalInterest { get; set; }

        [Column("loan_purpose")]
        public string? LoanPurpose { get; set; }

        [Column("applied_at")]
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
        public ICollection<RepaymentSchedule> RepaymentSchedules { get; set; } = new List<RepaymentSchedule>();
    }
}