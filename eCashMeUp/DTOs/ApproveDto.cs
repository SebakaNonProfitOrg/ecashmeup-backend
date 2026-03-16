namespace eCashMeUp.DTOs
{
    public class ApproveDto
    {
        public short? CreditScore { get; set; }
        public decimal? DebtToIncomeRatio { get; set; }
        public decimal? AffordabilityAmount { get; set; }
        public string AssessedBy { get; set; } = "System";
    }
}