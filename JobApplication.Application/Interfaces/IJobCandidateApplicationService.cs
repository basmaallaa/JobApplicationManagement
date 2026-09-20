using JobApplication.Application.DTOs;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;

namespace JobApplication.Application.Interfaces
{
    public interface IJobCandidateApplicationService
    {
        IEnumerable<JobApplicationResponseDto> GetAll();
        Task<JobApplicationResponseDto> CreateAsync(CreateJobCandidateApplicationDto createJobApplicationDto);
        Task<JobApplicationResponseDto> Review(int id);
        Task<JobApplicationResponseDto> UpdateStatus(int id, JobApplicationStatus status);
        Task<JobApplicationResponseDto> Cancel(int id);
    }
}
