using JobApplication.Application.DTOs;
using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace JobApplication.Application.Features.Applications.Commands.UpdateApplicationStatus
{
    public class UpdateApplicationStatusCommandHandler : IRequestHandler<UpdateApplicationStatusCommand, JobApplicationResponseDto>
    {
        private readonly IRepository<JobCandidateApplication> _jobApplicationRepository;
        private readonly IRepository<Job> _jobRepository;
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

        public UpdateApplicationStatusCommandHandler(
            IRepository<JobCandidateApplication> jobApplicationRepository,
            IRepository<Job> jobRepository,
            ICurrentUser currentUser)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _currentUser = currentUser;
        }

        public async Task<JobApplicationResponseDto> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
        {
            var recruiterId = _currentUser.RecruiterId
                ?? throw new ForbiddenException("Only recruiters can perform this action.");

            var jobApplication = _jobApplicationRepository.Get().FirstOrDefault(a => a.Id == request.Id)
                ?? throw new NotFoundException("Application not found.");

            var ownsJob = _jobRepository.Get().Any(j => j.Id == jobApplication.JobId && j.RecruiterId == recruiterId);
            if (!ownsJob)
            {
                throw new ForbiddenException("You can only manage applications for your own jobs.");
            }

            jobApplication.UpdateStatus(request.Status);
            await _jobApplicationRepository.SaveChangesAsync();

            return _jobApplicationRepository.Get()
                .Where(a => a.Id == request.Id)
                .Select(ToResponse)
                .First();
        }
    }
}
