using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobApplication.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email?.Trim() ?? string.Empty);

            if (user == null ||
                string.IsNullOrEmpty(request.Password) ||
                !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                ?? throw new UnauthorizedException("User has no role.");

            return _tokenService.CreateToken(user, role);
        }
    }
}
