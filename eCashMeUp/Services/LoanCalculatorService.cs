namespace eCashMeUp.Services
{
    public class LoanCalculatorService
    {
        private const decimal AnnualRate = 15m;

        public object Calculate(decimal loanAmount, int termMonths)
        {
            decimal monthlyRate = AnnualRate / 100m / 12m;

            double factor = Math.Pow((double)(1 + monthlyRate), termMonths);
            decimal monthlyPayment = loanAmount
                                   * (decimal)(factor * (double)monthlyRate)
                                   / (decimal)(factor - 1);

            monthlyPayment = Math.Round(monthlyPayment, 2);

            decimal totalRepayable = Math.Round(monthlyPayment * termMonths, 2);
            decimal totalInterest = Math.Round(totalRepayable - loanAmount, 2);

            var schedule = new List<object>();
            decimal balance = loanAmount;

            for (int i = 1; i <= termMonths; i++)
            {
                decimal interest = Math.Round(balance * monthlyRate, 2);
                decimal principal = Math.Round(monthlyPayment - interest, 2);
                balance = Math.Round(balance - principal, 2);
                if (balance < 0) balance = 0;

                schedule.Add(new
                {
                    Month = i,
                    DueDate = DateTime.Today.AddMonths(i).ToString("yyyy-MM-dd"),
                    Installment = monthlyPayment,
                    OutstandingBalance = balance
                });
            }

            return new
            {
                LoanAmount = loanAmount,
                TermMonths = termMonths,
                InterestRate = $"{AnnualRate}% per annum",
                MonthlyInstallment = monthlyPayment,
                TotalRepayable = totalRepayable,
                TotalInterest = totalInterest,
                Schedule = schedule
            };
        }
    }
}