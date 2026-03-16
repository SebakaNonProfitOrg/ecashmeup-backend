using eCashMeUp.DTOs;
using eCashMeUp.Services;
using Microsoft.AspNetCore.Mvc;

namespace eCashMeUp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, message, token) = await _authService.RegisterAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, token });
        }

        /// Login and receive JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, message, token) = await _authService.LoginAsync(dto);

            if (!success)
                return Unauthorized(new { message });

            return Ok(new { message, token });
        }
    }
}