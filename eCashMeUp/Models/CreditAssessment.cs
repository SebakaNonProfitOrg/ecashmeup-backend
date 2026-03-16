using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("credit_assessments")]
    public class CreditAssessment
    {
        [Key]
        [Column("assessment_id")]
        public int AssessmentId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }

        [Column("assessment_result")]
        public string AssessmentResult { get; set; } = "";

        [Column("credit_score")]
        public short? CreditScore { get; set; }

        [Column("debt_to_income_ratio")]
        public decimal? DebtToIncomeRatio { get; set; }

        [Column("affordability_amount")]
        public decimal? AffordabilityAmount { get; set; }

        [Column("decline_reason")]
        public string? DeclineReason { get; set; }

        [Column("assessed_by")]
        public string? AssessedBy { get; set; }

        [Column("assessed_at")]
        public DateTime AssessedAt { get; set; } = DateTime.UtcNow;
    }
}