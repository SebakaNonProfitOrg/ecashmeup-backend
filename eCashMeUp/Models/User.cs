using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("title_id")]
        public int TitleId { get; set; }

        [Column("race_id")]
        public int RaceId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = "";

        [Column("last_name")]
        public string LastName { get; set; } = "";

        [Column("id_number")]
        public string? IdNumber { get; set; }

        [Column("passport_number")]
        public string? PassportNumber { get; set; }

        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Column("phone")]
        public string Phone { get; set; } = "";

        [Column("email")]
        public string Email { get; set; } = "";

        [Column("password_hash")]
        public string PasswordHash { get; set; } = "";

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public EmploymentDetail? EmploymentDetail { get; set; }
        public FinancialDetail? FinancialDetail { get; set; }
        public BankingDetail? BankingDetail { get; set; }
        public ICollection<LoanApplication> LoanApplications { get; set; } = new List<LoanApplication>();
    }
}