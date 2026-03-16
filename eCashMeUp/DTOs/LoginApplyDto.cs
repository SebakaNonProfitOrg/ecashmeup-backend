namespace eCashMeUp.DTOs
{
    public class LoanApplyDto
    {
        // Loan
        public decimal LoanAmount { get; set; }
        public int TermMonths { get; set; }
        public string LoanPurpose { get; set; } = "";

        // Employment
        public string EmployerName { get; set; } = "";
        public string EmploymentType { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public decimal MonthlySalary { get; set; }
        public DateTime StartDate { get; set; }
        public string? EmployerPhone { get; set; }
        public string? EmployerAddress { get; set; }

        // Financial
        public decimal MonthlyGrossIncome { get; set; }
        public decimal MonthlyNetIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal OtherLoanObligations { get; set; }

        // Banking
        public string BankName { get; set; } = "";
        public string AccountHolder { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string BranchCode { get; set; } = "";
        public string AccountType { get; set; } = "";
    }
}