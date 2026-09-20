using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _JobService;

        public JobsController(IJobService jobService)
        {
            _JobService = jobService;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> Create(CreateJobDto createJobDto)
        {
            var id = await _JobService.CreateAsync(createJobDto);
            return Ok(new
            {
                id = id 
            }); 
        }

        [HttpPut("{id}/close")]
        [Authorize(Roles = nameof(UserRole.Recruiter))]
        public async Task<IActionResult> Close(int id)
        {
            await _JobService.Close(id);
            return Ok(new { id = id });
        }
    }
}
