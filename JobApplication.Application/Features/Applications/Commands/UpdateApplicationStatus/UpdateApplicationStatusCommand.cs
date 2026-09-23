using JobApplication.Application.DTOs;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.UpdateApplicationStatus
{
    public class UpdateApplicationStatusCommand : IRequest<JobApplicationResponseDto>
    {
        public int Id { get; set; }
        public JobApplicationStatus Status { get; set; }
    }
}
