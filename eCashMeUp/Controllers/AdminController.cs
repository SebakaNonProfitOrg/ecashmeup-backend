using eCashMeUp.Data;
using eCashMeUp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCashMeUp.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // Get all pending applications
        [HttpGet("applications")]
        public async Task<IActionResult> GetAllApplications()
        {
            var applications = await _db.LoanApplications
                .Include(l => l.User)
                .OrderByDescending(l => l.AppliedAt)
                .Select(l => new
                {
                    l.ApplicationId,
                    ApplicantName = $"{l.User!.FirstName} {l.User.LastName}",
                    ApplicantEmail = l.User.Email,
                    l.LoanAmount,
                    l.LoanTermMonths,
                    l.MonthlyInstallment,
                    l.TotalRepayable,
                    l.LoanPurpose,
                    l.StatusId,
                    l.AppliedAt
                })
                .ToListAsync();

            return Ok(applications);
        }

        // Get one application in full detail
        [HttpGet("applications/{id}")]
        public async Task<IActionResult> GetApplication(int id)
        {
            var loan = await _db.LoanApplications
                .Include(l => l.User)
                    .ThenInclude(u => u!.EmploymentDetail)
                .Include(l => l.User)
                    .ThenInclude(u => u!.FinancialDetail)
                .Include(l => l.User)
                    .ThenInclude(u => u!.BankingDetail)
                .Include(l => l.RepaymentSchedules)
                .FirstOrDefaultAsync(l => l.ApplicationId == id);

            if (loan == null)
                return NotFound(new { message = "Application not found." });

            return Ok(new
            {
                loan.ApplicationId,
                loan.LoanAmount,
                loan.LoanTermMonths,
                loan.MonthlyInstallment,
                loan.TotalRepayable,
                loan.TotalInterest,
                loan.LoanPurpose,
                loan.StatusId,
                loan.AppliedAt,

                Applicant = new
                {
                    loan.User!.FirstName,
                    loan.User.LastName,
                    loan.User.Email,
                    loan.User.Phone,
                    loan.User.IdNumber
                },

                Employment = loan.User.EmploymentDetail == null ? null : new
                {
                    loan.User.EmploymentDetail.EmployerName,
                    loan.User.EmploymentDetail.EmploymentType,
                    loan.User.EmploymentDetail.JobTitle,
                    loan.User.EmploymentDetail.MonthlySalary
                },

                Financial = loan.User.FinancialDetail == null ? null : new
                {
                    loan.User.FinancialDetail.MonthlyGrossIncome,
                    loan.User.FinancialDetail.MonthlyNetIncome,
                    loan.User.FinancialDetail.MonthlyExpenses,
                    loan.User.FinancialDetail.OtherLoanObligations,
                    loan.User.FinancialDetail.NetDisposableIncome
                },

                Banking = loan.User.BankingDetail == null ? null : new
                {
                    loan.User.BankingDetail.BankName,
                    loan.User.BankingDetail.AccountNumber,
                    loan.User.BankingDetail.AccountType
                },

                Schedule = loan.RepaymentSchedules.Select(s => new
                {
                    s.InstallmentNumber,
                    s.DueDate,
                    s.AmountDue,
                    s.OutstandingBalance,
                    s.IsPaid
                })
            });
        }

        // Move application to Under Review
        [HttpPut("applications/{id}/review")]
        public async Task<IActionResult> SetUnderReview(int id)
        {
            var loan = await _db.LoanApplications.FindAsync(id);
            if (loan == null)
                return NotFound(new { message = "Application not found." });

            if (loan.StatusId != 1)
                return BadRequest(new { message = "Application is not in Pending status." });

            loan.StatusId = 2; // Under Review
            loan.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Application is now Under Review." });
        }

        // Approve a loan application
        [HttpPut("applications/{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveDto dto)
        {
            var loan = await _db.LoanApplications.FindAsync(id);
            if (loan == null)
                return NotFound(new { message = "Application not found." });

            if (loan.StatusId != 2)
                return BadRequest(new { message = "Application must be Under Review before approving." });

            // Save credit assessment
            _db.CreditAssessments.Add(new Models.CreditAssessment
            {
                ApplicationId = id,
                AssessmentResult = "Approved",
                CreditScore = dto.CreditScore,
                DebtToIncomeRatio = dto.DebtToIncomeRatio,
                AffordabilityAmount = dto.AffordabilityAmount,
                AssessedBy = dto.AssessedBy,
                AssessedAt = DateTime.UtcNow
            });

            loan.StatusId = 3;
            loan.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Loan approved successfully." });
        }

        // Decline a loan application
        [HttpPut("applications/{id}/decline")]
        public async Task<IActionResult> Decline(int id, [FromBody] DeclineDto dto)
        {
            var loan = await _db.LoanApplications.FindAsync(id);
            if (loan == null)
                return NotFound(new { message = "Application not found." });

            if (loan.StatusId != 2)
                return BadRequest(new { message = "Application must be Under Review before declining." });

            _db.CreditAssessments.Add(new Models.CreditAssessment
            {
                ApplicationId = id,
                AssessmentResult = "Declined",
                DeclineReason = dto.Reason,
                AssessedBy = dto.AssessedBy,
                AssessedAt = DateTime.UtcNow
            });

            loan.StatusId = 4;
            loan.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Loan declined.", reason = dto.Reason });
        }

        // Disburse an approved loan
        [HttpPut("applications/{id}/disburse")]
        public async Task<IActionResult> Disburse(int id)
        {
            var loan = await _db.LoanApplications.FindAsync(id);
            if (loan == null)
                return NotFound(new { message = "Application not found." });

            if (loan.StatusId != 3)
                return BadRequest(new { message = "Loan must be Approved before disbursing." });

            // Create disbursement record
            _db.LoanDisbursements.Add(new Models.LoanDisbursement
            {
                ApplicationId = id,
                DisbursedAmount = loan.LoanAmount,
                DisbursementMethod = "EFT",
                ReferenceNumber = $"ECM-{id}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                DisbursedAt = DateTime.UtcNow
            });

            loan.StatusId = 5;
            loan.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Set to Active right after disbursement
            loan.StatusId = 6;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Loan disbursed successfully.",
                reference = $"ECM-{id}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                disbursedAmount = loan.LoanAmount
            });
        }
    }
}