using eCashMeUp.Data;
using eCashMeUp.DTOs;
using eCashMeUp.Helpers;
using eCashMeUp.Models;
using Microsoft.EntityFrameworkCore;

namespace eCashMeUp.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtHelper _jwt;

        public AuthService(AppDbContext db, JwtHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<(bool Success, string Message, string? Token)>
            RegisterAsync(RegisterDto dto)
        {
            // Age check
            var today = DateTime.Today;
            int age = today.Year - dto.DateOfBirth.Year;
            if (dto.DateOfBirth > today.AddYears(-age)) age--;
            if (age < 18)
                return (false, "You must be at least 18 years old.", null);

            // Name check — no numbers allowed
            if (dto.FirstName.Any(char.IsDigit) || dto.LastName.Any(char.IsDigit))
                return (false, "First name and last name cannot contain numbers.", null);

            // ID or Passport required
            if (string.IsNullOrWhiteSpace(dto.IdNumber) &&
                string.IsNullOrWhiteSpace(dto.PassportNumber))
                return (false, "Please provide an ID number or Passport number.", null);

            // Duplicate email check
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return (false, "This email is already registered.", null);

            // Save user
            var user = new User
            {
                TitleId = dto.TitleId,
                RaceId = dto.RaceId,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim().ToLower(),
                Phone = dto.Phone.Trim(),
                DateOfBirth = dto.DateOfBirth,
                IdNumber = dto.IdNumber,
                PassportNumber = dto.PassportNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwt.GenerateToken(user);
            return (true, "Registration successful.", token);
        }

        public async Task<(bool Success, string Message, string? Token)>
            LoginAsync(LoginDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, "Invalid email or password.", null);

            if (!user.IsActive)
                return (false, "Your account has been deactivated.", null);

            var token = _jwt.GenerateToken(user);
            return (true, "Login successful.", token);
        }
    }
}