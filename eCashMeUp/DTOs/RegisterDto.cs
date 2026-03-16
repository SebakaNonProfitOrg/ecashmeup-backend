namespace eCashMeUp.DTOs
{
    public class RegisterDto
    {
        public int TitleId { get; set; }
        public int RaceId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string? IdNumber { get; set; }
        public string? PassportNumber { get; set; }
    }
}