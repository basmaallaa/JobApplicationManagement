using JobApplication.Application.DTOs;
using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly ICurrentUser _currentUser;

        public JobService(IRepository<Job> jobRepository, ICurrentUser currentUser)
        {
            _jobRepository = jobRepository;
            _currentUser = currentUser;
        }

        public async Task<int> CreateAsync(CreateJobDto createJobDto)
        {   
            var recruiterId = _currentUser.RecruiterId
                ?? throw new ForbiddenException("Only recruiters can create jobs.");

            var job = new Job()
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                IsActive = true,
                RecruiterId = recruiterId
            };
            await _jobRepository.AddAsync(job);
            await _jobRepository.SaveChangesAsync();

            return job.Id; 
        }

        public async Task Close(int id)
        {
            var recruiterId = _currentUser.RecruiterId
                ?? throw new ForbiddenException("Only recruiters can close jobs.");

            var job = _jobRepository.Get().FirstOrDefault(j => j.Id == id)
                ?? throw new NotFoundException("Job not found.");

            if (job.RecruiterId != recruiterId)
            {
                throw new ForbiddenException("You can only close your own jobs.");
            }

            job.Close(recruiterId);
            await _jobRepository.SaveChangesAsync();
        }
    }
}
