using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobApplication.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, int>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<int> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BadRequestException("Email, password and name are required.");
            }

            if (!Enum.IsDefined(request.Role))
            {
                throw new BadRequestException("Role must be Candidate or Recruiter.");
            }

            var roleName = request.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }

            var user = new User { UserName = email, Email = email };

            // Candidate/Recruiter is created together with the user in a single SaveChanges.
            if (request.Role == UserRole.Candidate)
            {
                user.Candidate = new Candidate { Name = request.Name.Trim(), CvUrl = request.CvUrl ?? string.Empty };
            }
            else
            {
                user.Recruiter = new Recruiter { Name = request.Name.Trim() };
            }

            var result = await _userManager.CreateAsync(user, request.Password);
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
    }
}
