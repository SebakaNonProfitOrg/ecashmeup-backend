using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("employment_details")]
    public class EmploymentDetail
    {
        [Key]
        [Column("employment_id")]
        public int EmploymentId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("employer_name")]
        public string EmployerName { get; set; } = "";

        [Column("employment_type")]
        public string EmploymentType { get; set; } = "";

        [Column("job_title")]
        public string JobTitle { get; set; } = "";

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("employer_phone")]
        public string? EmployerPhone { get; set; }

        [Column("employer_address")]
        public string? EmployerAddress { get; set; }

        [Column("monthly_salary")]
        public decimal MonthlySalary { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}