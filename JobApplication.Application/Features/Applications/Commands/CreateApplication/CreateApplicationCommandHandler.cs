using JobApplication.Application.DTOs;
using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace JobApplication.Application.Features.Applications.Commands.CreateApplication
{
    public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, JobApplicationResponseDto>
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

        public CreateApplicationCommandHandler(
            IRepository<JobCandidateApplication> jobApplicationRepository,
            IRepository<Job> jobRepository,
            ICurrentUser currentUser)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _currentUser = currentUser;
        }

        public async Task<JobApplicationResponseDto> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var candidateId = _currentUser.CandidateId
                ?? throw new ForbiddenException("Only candidates can apply for jobs.");

            var job = _jobRepository.Get().FirstOrDefault(j => j.Id == request.JobId)
                ?? throw new NotFoundException("Job not found.");

            if (!job.IsActive)
            {
                throw new BadRequestException("Job is closed.");
            }

            var jobApplication = new JobCandidateApplication()
            {
                JobId = request.JobId,
                CandidateId = candidateId,
            };

            await _jobApplicationRepository.AddAsync(jobApplication);
            await _jobApplicationRepository.SaveChangesAsync();

            return _jobApplicationRepository.Get()
                .Where(a => a.Id == jobApplication.Id)
                .Select(ToResponse)
                .First();
        }
    }
}
