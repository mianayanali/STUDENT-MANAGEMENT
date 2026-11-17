using STUDENT_MANAGEMENT.DTOs.Auth;

namespace STUDENT_MANAGEMENT.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDTO dto);
        Task<string?> LoginAsync(LoginDTO dto);
    }
}

