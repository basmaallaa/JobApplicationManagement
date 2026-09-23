using JobApplication.Application.DTOs;
using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.ReviewApplication
{
    public class ReviewApplicationCommand : IRequest<JobApplicationResponseDto>
    {
        public int Id { get; set; }
    }
}
