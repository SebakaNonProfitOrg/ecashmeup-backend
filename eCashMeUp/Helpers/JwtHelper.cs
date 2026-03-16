using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eCashMeUp.Models;
using Microsoft.IdentityModel.Tokens;

namespace eCashMeUp.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId",           user.UserId.ToString()),
                new Claim(ClaimTypes.Email,   user.Email),
                new Claim(ClaimTypes.Name,    $"{user.FirstName} {user.LastName}")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}