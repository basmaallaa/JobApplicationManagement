using JobApplication.Application.DTOs;
using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace JobApplication.Application.Services
{
    public class JobCandidateApplicationService : IJobCandidateApplicationService
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

        public JobCandidateApplicationService(
            IRepository<JobCandidateApplication> jobApplicationRepository,
            IRepository<Job> jobRepository,
            ICurrentUser currentUser)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _currentUser = currentUser;
        }

        public IEnumerable<JobApplicationResponseDto> GetAll()
        {
            return _jobApplicationRepository.Get().Select(ToResponse).ToList();
        }

        public async Task<JobApplicationResponseDto> CreateAsync(CreateJobCandidateApplicationDto createJobApplicationDto)
        {
            var candidateId = _currentUser.CandidateId
                ?? throw new ForbiddenException("Only candidates can apply for jobs.");

            var job = _jobRepository.Get().FirstOrDefault(j => j.Id == createJobApplicationDto.JobId)
                ?? throw new NotFoundException("Job not found.");
            if (!job.IsActive)
            {
                throw new BadRequestException("Job is closed.");
            }

            var jobApplication = new JobCandidateApplication()
            {
                JobId = createJobApplicationDto.JobId,
                CandidateId = candidateId,
            };
            await _jobApplicationRepository.AddAsync(jobApplication);
            await _jobApplicationRepository.SaveChangesAsync();
            return GetResponse(jobApplication.Id);
        }

        public async Task<JobApplicationResponseDto> Review(int id)
        {
            var jobApplication = GetOwnedByRecruiter(id);
            jobApplication.Review();
            await _jobApplicationRepository.SaveChangesAsync();
            return GetResponse(id);
        }

        public async Task<JobApplicationResponseDto> UpdateStatus(int id, JobApplicationStatus status)
        {
            var jobApplication = GetOwnedByRecruiter(id);
            jobApplication.UpdateStatus(status);
            await _jobApplicationRepository.SaveChangesAsync();
            return GetResponse(id);
        }

        public async Task<JobApplicationResponseDto> Cancel(int id)
        {
            var candidateId = _currentUser.CandidateId
                ?? throw new ForbiddenException("Only candidates can cancel applications.");

            var jobApplication = _jobApplicationRepository.Get().FirstOrDefault(a => a.Id == id)
                ?? throw new NotFoundException("Application not found.");

            if (jobApplication.CandidateId != candidateId)
            {
                throw new ForbiddenException("You can only cancel your own applications.");
            }

            jobApplication.Cancel();
            await _jobApplicationRepository.SaveChangesAsync();
            return GetResponse(id);
        }

        private JobApplicationResponseDto GetResponse(int id)
        {
            return _jobApplicationRepository.Get().Where(a => a.Id == id).Select(ToResponse).First();
        }

        // Loads the application and verifies the authenticated recruiter owns its job.
        private JobCandidateApplication GetOwnedByRecruiter(int id)
        {
            var recruiterId = _currentUser.RecruiterId
                ?? throw new ForbiddenException("Only recruiters can perform this action.");

            var jobApplication = _jobApplicationRepository.Get().FirstOrDefault(a => a.Id == id)
                ?? throw new NotFoundException("Application not found.");

            var ownsJob = _jobRepository.Get().Any(j => j.Id == jobApplication.JobId && j.RecruiterId == recruiterId);
            if (!ownsJob)
            {
                throw new ForbiddenException("You can only manage applications for your own jobs.");
            }

            return jobApplication;
        }
    }
}