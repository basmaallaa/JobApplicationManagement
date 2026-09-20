using JobApplication.Application.DTOs;

namespace JobApplication.Application.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
