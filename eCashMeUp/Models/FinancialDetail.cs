using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("financial_details")]
    public class FinancialDetail
    {
        [Key]
        [Column("financial_id")]
        public int FinancialId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("monthly_gross_income")]
        public decimal MonthlyGrossIncome { get; set; }

        [Column("monthly_net_income")]
        public decimal MonthlyNetIncome { get; set; }

        [Column("monthly_expenses")]
        public decimal MonthlyExpenses { get; set; }

        [Column("other_loan_obligations")]
        public decimal OtherLoanObligations { get; set; }

        [Column("net_disposable_income")]
        public decimal NetDisposableIncome { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}