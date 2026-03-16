using eCashMeUp.DTOs;
using eCashMeUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCashMeUp.Controllers
{
    [ApiController]
    [Route("api/loan")]
    public class LoanController : ControllerBase
    {
        private readonly LoanService _loanService;
        private readonly LoanCalculatorService _calculator;

        public LoanController(LoanService loanService,
                              LoanCalculatorService calculator)
        {
            _loanService = loanService;
            _calculator = calculator;
        }

        /// Calculate loan repayments — no login required
        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] LoanCalculateDto dto)
        {
            if (dto.LoanAmount < 100 || dto.LoanAmount > 3000)
                return BadRequest(new { message = "Loan amount must be between R100 and R3,000." });

            if (dto.TermMonths < 1 || dto.TermMonths > 3)
                return BadRequest(new { message = "Term must be 1 to 3 months." });

            var result = _calculator.Calculate(dto.LoanAmount, dto.TermMonths);
            return Ok(result);
        }

        /// Submit a loan application
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply([FromBody] LoanApplyDto dto)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var (success, message, data) = await _loanService.ApplyAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, data });
        }

        /// Get a loan by ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetLoan(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null)
                return NotFound(new { message = "Loan not found." });

            return Ok(loan);
        }

        /// Get all loans for the logged-in user
        [HttpGet("my-loans")]
        [Authorize]
        public async Task<IActionResult> GetMyLoans()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var loans = await _loanService.GetUserLoansAsync(userId);
            return Ok(loans);
        }
    }
}