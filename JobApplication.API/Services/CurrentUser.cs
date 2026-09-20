using JobApplication.Application.Interfaces;
using JobApplication.Domain.Enums;

namespace JobApplication.API.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly System.Security.Claims.ClaimsPrincipal? _user;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext?.User;
        }

        public int? UserId => GetInt("sub");
        public int? CandidateId => GetInt("candidateId");
        public int? RecruiterId => GetInt("recruiterId");
        public UserRole? Role =>
            Enum.TryParse<UserRole>(_user?.FindFirst("role")?.Value, out var role) ? role : null;

        private int? GetInt(string claimType) =>
            int.TryParse(_user?.FindFirst(claimType)?.Value, out var value) ? value : null;
    }
}
