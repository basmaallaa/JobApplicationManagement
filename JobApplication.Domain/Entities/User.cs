using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplication.Domain.Entities
{
    // ASP.NET Core Identity user (int key). Id, Email, UserName, PasswordHash come from IdentityUser<int>.
    // The role (Candidate / Recruiter) is stored in the Identity roles tables.
    public class User : IdentityUser<int>
    {
        public int? CandidateId { get; set; }
        [ForeignKey(nameof(CandidateId))]
        public Candidate? Candidate { get; set; }

        public int? RecruiterId { get; set; }
        [ForeignKey(nameof(RecruiterId))]
        public Recruiter? Recruiter { get; set; }
    }
}
