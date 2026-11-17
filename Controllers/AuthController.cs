using Microsoft.AspNetCore.Mvc;
using STUDENT_MANAGEMENT.DTOs.Auth;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register - Naya user register karna
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var success = await _authService.RegisterAsync(dto);
            if (!success)
            {
                return Conflict("Username is already taken.");
            }

            return Ok(new { message = "User registered successfully." });
        }

        // POST: api/auth/login - Login karke JWT token hasil karna
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(new
            {
                message = "Login successful.",
                token = token
            });
        }
    }
}

