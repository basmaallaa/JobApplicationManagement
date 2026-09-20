using JobApplication.Domain.Enums;

namespace JobApplication.Application.DTOs
{
    public class UpdateApplicationStatusDto
    {
        public JobApplicationStatus Status { get; set; }
    }
}
