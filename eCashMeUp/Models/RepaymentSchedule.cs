using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("repayment_schedules")]
    public class RepaymentSchedule
    {
        [Key]
        [Column("schedule_id")]
        public int ScheduleId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }

        [Column("installment_number")]
        public int InstallmentNumber { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("amount_due")]
        public decimal AmountDue { get; set; }

        [Column("outstanding_balance")]
        public decimal OutstandingBalance { get; set; }

        [Column("is_paid")]
        public bool IsPaid { get; set; } = false;

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        // Navigation
        public LoanApplication? LoanApplication { get; set; }
    }
}