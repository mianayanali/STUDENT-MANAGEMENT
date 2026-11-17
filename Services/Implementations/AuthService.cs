using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.DTOs.Auth;
using STUDENT_MANAGEMENT.Models;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            // Check duplicate username
            var userExists = await _context.Users.AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());
            if (userExists)
            {
                return false;
            }

            // Password hash karna using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new AppUser
            {
                Username = dto.Username,
                PasswordHash = passwordHash,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "Staff" : dto.Role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());
            if (user == null)
            {
                return null; // User nahi mila
            }

            // Password verify karna
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return null; // Invalid credentials
            }

            // JWT Token generate karna
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(AppUser user)
        {
            var keyString = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var duration = int.Parse(_config["Jwt:DurationInMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

