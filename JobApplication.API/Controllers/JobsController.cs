using JobApplication.Application.DTOs;
using JobApplication.Application.Features.Jobs.Commands.CloseJob;
using JobApplication.Application.Features.Jobs.Commands.CreateJob;
using JobApplication.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> Create(CreateJobDto createJobDto)
        {
            var command = new CreateJobCommand
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description
            };
            var id = await _mediator.Send(command);
            return Ok(new { id = id });
        }

        [HttpPut("{id}/close")]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> Close(int id)
        {
            await _mediator.Send(new CloseJobCommand { Id = id });
            return Ok(new { id = id });
        }
    }
}
