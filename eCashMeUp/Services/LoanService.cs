using eCashMeUp.Data;
using eCashMeUp.DTOs;
using eCashMeUp.Models;
using Microsoft.EntityFrameworkCore;

namespace eCashMeUp.Services
{
    public class LoanService
    {
        private readonly AppDbContext _db;
        private readonly LoanCalculatorService _calculator;

        public LoanService(AppDbContext db, LoanCalculatorService calculator)
        {
            _db = db;
            _calculator = calculator;
        }

        public async Task<(bool Success, string Message, object? Data)>
            ApplyAsync(int userId, LoanApplyDto dto)
        {
            // 1. Validate loan amount and term
            if (dto.LoanAmount < 100 || dto.LoanAmount > 3000)
                return (false, "Loan amount must be between R100 and R3,000.", null);

            if (dto.TermMonths < 1 || dto.TermMonths > 3)
                return (false, "Loan term must be between 1 and 3 months.", null);

            // 2. Calculate repayments
            var calc = (dynamic)_calculator.Calculate(dto.LoanAmount, dto.TermMonths);

            // 3. Save or update employment details
            var employment = await _db.EmploymentDetails
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employment == null)
            {
                employment = new EmploymentDetail { UserId = userId };
                _db.EmploymentDetails.Add(employment);
            }

            employment.EmployerName = dto.EmployerName;
            employment.EmploymentType = dto.EmploymentType;
            employment.JobTitle = dto.JobTitle;
            employment.MonthlySalary = dto.MonthlySalary;
            employment.StartDate = dto.StartDate;
            employment.EmployerPhone = dto.EmployerPhone;
            employment.EmployerAddress = dto.EmployerAddress;

            // 4. Save or update financial details
            var financial = await _db.FinancialDetails
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (financial == null)
            {
                financial = new FinancialDetail { UserId = userId };
                _db.FinancialDetails.Add(financial);
            }

            financial.MonthlyGrossIncome = dto.MonthlyGrossIncome;
            financial.MonthlyNetIncome = dto.MonthlyNetIncome;
            financial.MonthlyExpenses = dto.MonthlyExpenses;
            financial.OtherLoanObligations = dto.OtherLoanObligations;

            // 5. Save or update banking details
            var banking = await _db.BankingDetails
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (banking == null)
            {
                banking = new BankingDetail { UserId = userId };
                _db.BankingDetails.Add(banking);
            }

            banking.BankName = dto.BankName;
            banking.AccountHolder = dto.AccountHolder;
            banking.AccountNumber = dto.AccountNumber;
            banking.BranchCode = dto.BranchCode;
            banking.AccountType = dto.AccountType;

            // 6. Create loan application
            var application = new LoanApplication
            {
                UserId = userId,
                StatusId = 1, // Pending
                LoanAmount = dto.LoanAmount,
                LoanTermMonths = dto.TermMonths,
                InterestRate = 15.00m,
                MonthlyInstallment = calc.MonthlyInstallment,
                TotalRepayable = calc.TotalRepayable,
                TotalInterest = calc.TotalInterest,
                LoanPurpose = dto.LoanPurpose,
                AppliedAt = DateTime.UtcNow
            };

            _db.LoanApplications.Add(application);
            await _db.SaveChangesAsync();

            // 7. Generate repayment schedule
            for (int i = 1; i <= dto.TermMonths; i++)
            {
                var scheduleItem = (dynamic)((List<object>)calc.Schedule)[i - 1];
                _db.RepaymentSchedules.Add(new RepaymentSchedule
                {
                    ApplicationId = application.ApplicationId,
                    InstallmentNumber = i,
                    DueDate = DateTime.Today.AddMonths(i),
                    AmountDue = calc.MonthlyInstallment,
                    OutstandingBalance = scheduleItem.OutstandingBalance
                });
            }

            await _db.SaveChangesAsync();

            return (true, "Loan application submitted successfully.", new
            {
                application.ApplicationId,
                application.LoanAmount,
                application.LoanTermMonths,
                application.MonthlyInstallment,
                application.TotalRepayable,
                application.TotalInterest,
                Status = "Pending"
            });
        }

        public async Task<object?> GetByIdAsync(int applicationId)
        {
            var loan = await _db.LoanApplications
                .Include(l => l.RepaymentSchedules)
                .FirstOrDefaultAsync(l => l.ApplicationId == applicationId);

            if (loan == null) return null;

            return new
            {
                loan.ApplicationId,
                loan.LoanAmount,
                loan.LoanTermMonths,
                loan.InterestRate,
                loan.MonthlyInstallment,
                loan.TotalRepayable,
                loan.TotalInterest,
                loan.LoanPurpose,
                loan.StatusId,
                loan.AppliedAt,
                Schedule = loan.RepaymentSchedules.Select(s => new
                {
                    s.InstallmentNumber,
                    s.DueDate,
                    s.AmountDue,
                    s.OutstandingBalance,
                    s.IsPaid,
                    s.PaidAt
                })
            };
        }

        public async Task<object> GetUserLoansAsync(int userId)
        {
            var loans = await _db.LoanApplications
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.AppliedAt)
                .Select(l => new
                {
                    l.ApplicationId,
                    l.LoanAmount,
                    l.LoanTermMonths,
                    l.MonthlyInstallment,
                    l.TotalRepayable,
                    l.StatusId,
                    l.AppliedAt
                })
                .ToListAsync();

            return loans;
        }
    }
}