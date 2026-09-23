using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace JobApplication.Application.Features.Applications.Queries.GetAllApplications
{
    public class GetAllApplicationsQueryHandler : IRequestHandler<GetAllApplicationsQuery, IEnumerable<JobApplicationResponseDto>>
    {
        private readonly IRepository<JobCandidateApplication> _jobApplicationRepository;

        // One projection used by every method, so all of them return the same response shape.
        private static readonly Expression<Func<JobCandidateApplication, JobApplicationResponseDto>> ToResponse =
            a => new JobApplicationResponseDto
            {
                Id = a.Id,
                CandidateId = a.CandidateId,
                CandidateName = a.Candidate.Name,
                JobId = a.JobId,
                JobTitle = a.Job.Title,
                JobApplicationStatus = a.JobApplicationStatus,
                AppliedAt = a.AppliedAt,
                StatusUpdatedAt = a.StatusUpdatedAt,
                CancelledAt = a.CancelledAt
            };

        public GetAllApplicationsQueryHandler(IRepository<JobCandidateApplication> jobApplicationRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
        }

        public Task<IEnumerable<JobApplicationResponseDto>> Handle(GetAllApplicationsQuery request, CancellationToken cancellationToken)
        {
            var result = _jobApplicationRepository.Get().Select(ToResponse).ToList().AsEnumerable();
            return Task.FromResult(result);
        }
    }
}
