using eCashMeUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCashMeUp.Controllers
{
    [ApiController]
    [Route("api/repayments")]
    [Authorize]
    public class RepaymentController : ControllerBase
    {
        private readonly RepaymentService _repaymentService;

        public RepaymentController(RepaymentService repaymentService)
        {
            _repaymentService = repaymentService;
        }

        // Get repayment schedule for a loan
        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetSchedule(int loanId)
        {
            var schedule = await _repaymentService.GetScheduleAsync(loanId);
            if (schedule == null)
                return NotFound(new { message = "No repayment schedule found." });

            return Ok(schedule);
        }
    }
}