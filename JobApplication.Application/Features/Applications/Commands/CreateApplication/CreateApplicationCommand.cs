using JobApplication.Application.DTOs;
using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.CreateApplication
{
    public class CreateApplicationCommand : IRequest<JobApplicationResponseDto>
    {
        public int JobId { get; set; }
    }
}
