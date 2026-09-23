using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<int>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? CvUrl { get; set; }
    }
}
