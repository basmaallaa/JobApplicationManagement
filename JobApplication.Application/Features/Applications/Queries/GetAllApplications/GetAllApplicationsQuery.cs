using JobApplication.Application.DTOs;
using MediatR;

namespace JobApplication.Application.Features.Applications.Queries.GetAllApplications
{
    public class GetAllApplicationsQuery : IRequest<IEnumerable<JobApplicationResponseDto>>
    {
    }
}
