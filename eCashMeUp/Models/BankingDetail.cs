using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCashMeUp.Models
{
    [Table("banking_details")]
    public class BankingDetail
    {
        [Key]
        [Column("banking_id")]
        public int BankingId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("bank_name")]
        public string BankName { get; set; } = "";

        [Column("account_holder")]
        public string AccountHolder { get; set; } = "";

        [Column("account_number")]
        public string AccountNumber { get; set; } = "";

        [Column("branch_code")]
        public string BranchCode { get; set; } = "";

        [Column("account_type")]
        public string AccountType { get; set; } = "";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}