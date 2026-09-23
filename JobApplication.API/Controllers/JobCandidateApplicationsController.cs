using JobApplication.Application.DTOs;
using JobApplication.Application.Features.Applications.Commands.CancelApplication;
using JobApplication.Application.Features.Applications.Commands.CreateApplication;
using JobApplication.Application.Features.Applications.Commands.ReviewApplication;
using JobApplication.Application.Features.Applications.Commands.UpdateApplicationStatus;
using JobApplication.Application.Features.Applications.Queries.GetAllApplications;
using JobApplication.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/applications")]
    [ApiController]
    [Authorize]
    public class JobCandidateApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobCandidateApplicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var applications = await _mediator.Send(new GetAllApplicationsQuery());
            return Ok(new { applications });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var applications = await _mediator.Send(new GetAllApplicationsQuery());
            var application = applications.FirstOrDefault(j => j.Id == id);
            if (application is null) return NotFound(new { message = "invalid Id" });
            return Ok(new { application });
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Candidate))]
        public async Task<IActionResult> Create(CreateJobCandidateApplicationDto createApplicationDto)
        {
            var application = await _mediator.Send(new CreateApplicationCommand { JobId = createApplicationDto.JobId });
            return Ok(new { application });
        }

        [HttpPut("{id}/review")]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> Review(int id)
        {
            var application = await _mediator.Send(new ReviewApplicationCommand { Id = id });
            return Ok(new { application });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> UpdateStatus(int id, UpdateApplicationStatusDto dto)
        {
            var application = await _mediator.Send(new UpdateApplicationStatusCommand { Id = id, Status = dto.Status });
            return Ok(new { application });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Candidate))]
        public async Task<IActionResult> Cancel(int id)
        {
            var application = await _mediator.Send(new CancelApplicationCommand { Id = id });
            return Ok(new { application });
        }
    }
}