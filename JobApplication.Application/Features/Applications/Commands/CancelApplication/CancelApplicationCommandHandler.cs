using JobApplication.Application.DTOs;
using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace JobApplication.Application.Features.Applications.Commands.CancelApplication
{
    public class CancelApplicationCommandHandler : IRequestHandler<CancelApplicationCommand, JobApplicationResponseDto>
    {
        private readonly IRepository<JobCandidateApplication> _jobApplicationRepository;
        private readonly ICurrentUser _currentUser;

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

        public CancelApplicationCommandHandler(
            IRepository<JobCandidateApplication> jobApplicationRepository,
            ICurrentUser currentUser)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _currentUser = currentUser;
        }

        public async Task<JobApplicationResponseDto> Handle(CancelApplicationCommand request, CancellationToken cancellationToken)
        {
            var candidateId = _currentUser.CandidateId
                ?? throw new ForbiddenException("Only candidates can cancel applications.");

            var jobApplication = _jobApplicationRepository.Get().FirstOrDefault(a => a.Id == request.Id)
                ?? throw new NotFoundException("Application not found.");

            if (jobApplication.CandidateId != candidateId)
            {
                throw new ForbiddenException("You can only cancel your own applications.");
            }

            jobApplication.Cancel();
            await _jobApplicationRepository.SaveChangesAsync();

            return _jobApplicationRepository.Get()
                .Where(a => a.Id == request.Id)
                .Select(ToResponse)
                .First();
        }
    }
}
