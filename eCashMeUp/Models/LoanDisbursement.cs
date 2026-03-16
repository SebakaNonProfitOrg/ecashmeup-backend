using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("loan_disbursements")]
    public class LoanDisbursement
    {
        [Key]
        [Column("disbursement_id")]
        public int DisbursementId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }

        [Column("disbursed_amount")]
        public decimal DisbursedAmount { get; set; }

        [Column("disbursement_method")]
        public string DisbursementMethod { get; set; } = "EFT";

        [Column("reference_number")]
        public string ReferenceNumber { get; set; } = "";

        [Column("disbursed_at")]
        public DateTime DisbursedAt { get; set; } = DateTime.UtcNow;
    }
}