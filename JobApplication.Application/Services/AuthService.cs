using JobApplication.Application.DTOs;
using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace JobApplication.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<int> RegisterAsync(RegisterDto registerDto)
        {
            var email = registerDto.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(registerDto.Password) ||
                string.IsNullOrWhiteSpace(registerDto.Name))
            {
                throw new BadRequestException("Email, password and name are required.");
            }
            if (!Enum.IsDefined(registerDto.Role))
            {
                throw new BadRequestException("Role must be Candidate or Recruiter.");
            }

            var roleName = registerDto.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }

            var user = new User { UserName = email, Email = email };

            // Candidate/Recruiter is created together with the user in a single SaveChanges.
            if (registerDto.Role == UserRole.Candidate)
            {
                user.Candidate = new Candidate { Name = registerDto.Name.Trim(), CvUrl = registerDto.CvUrl ?? string.Empty };
            }
            else
            {
                user.Recruiter = new Recruiter { Name = registerDto.Name.Trim() };
            }

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new BadRequestException(string.Join(" ", roleResult.Errors.Select(e => e.Description)));
            }

            return user.Id;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email?.Trim() ?? string.Empty);

            if (user == null ||
                string.IsNullOrEmpty(loginDto.Password) ||
                !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                ?? throw new UnauthorizedException("User has no role.");

            return _tokenService.CreateToken(user, role);
        }
    }
}
