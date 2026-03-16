using eCashMeUp.Data;
using Microsoft.EntityFrameworkCore;

namespace eCashMeUp.Services
{
    public class RepaymentService
    {
        private readonly AppDbContext _db;

        public RepaymentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<object?> GetScheduleAsync(int applicationId)
        {
            var schedules = await _db.RepaymentSchedules
                .Where(s => s.ApplicationId == applicationId)
                .OrderBy(s => s.InstallmentNumber)
                .ToListAsync();

            if (!schedules.Any()) return null;

            return schedules.Select(s => new
            {
                s.ScheduleId,
                s.InstallmentNumber,
                DueDate = s.DueDate.ToString("yyyy-MM-dd"),
                s.AmountDue,
                s.OutstandingBalance,
                s.IsPaid,
                PaidAt = s.PaidAt?.ToString("yyyy-MM-dd HH:mm")
            });
        }
    }
}