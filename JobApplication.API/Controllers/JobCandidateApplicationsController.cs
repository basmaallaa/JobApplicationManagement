using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/applications")]
    [ApiController]
    [Authorize]
    public class JobCandidateApplicationsController : ControllerBase
    {
        private readonly IJobCandidateApplicationService _JobCandidateApplicationService;

        public JobCandidateApplicationsController(IJobCandidateApplicationService jobApplicationService)
        {
            _JobCandidateApplicationService = jobApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var applications = _JobCandidateApplicationService.GetAll();
            return Ok(new { applications });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var application = _JobCandidateApplicationService.GetAll().FirstOrDefault(j => j.Id == id);
            if (application is null) return NotFound(new
            {
                message = "invalid Id"
            });
            return Ok(new { application });
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Candidate))]
        public async Task<IActionResult> Create(CreateJobCandidateApplicationDto createApplicationDto)
        {
            var application = await _JobCandidateApplicationService.CreateAsync(createApplicationDto);
            return Ok(new { application });
        }

        [HttpPut("{id}/review")]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> Review(int id)
        {
            var application = await _JobCandidateApplicationService.Review(id);
            return Ok(new { application });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> UpdateStatus(int id, UpdateApplicationStatusDto dto)
        {
            var application = await _JobCandidateApplicationService.UpdateStatus(id, dto.Status);
            return Ok(new { application });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Candidate))]
        public async Task<IActionResult> Cancel(int id)
        {
            var application = await _JobCandidateApplicationService.Cancel(id);
            return Ok(new { application });
        }
    }
}