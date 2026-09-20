using JobApplication.Domain.Enums;

namespace JobApplication.Application.Interfaces
{
    // Identity of the authenticated caller, taken from the validated JWT (never from the request body).
    public interface ICurrentUser
    {
        int? UserId { get; }
        UserRole? Role { get; }
        int? CandidateId { get; }
        int? RecruiterId { get; }
    }
}
