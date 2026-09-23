using JobApplication.Application.DTOs;
using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.CancelApplication
{
    public class CancelApplicationCommand : IRequest<JobApplicationResponseDto>
    {
        public int Id { get; set; }
    }
}
