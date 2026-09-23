using JobApplication.Application.DTOs;
using JobApplication.Application.Features.Auth.Commands.Login;
using JobApplication.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var command = new RegisterCommand
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                Name = registerDto.Name,
                Role = registerDto.Role,
                CvUrl = registerDto.CvUrl
            };
            var id = await _mediator.Send(command);
            return Ok(new { id = id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var command = new LoginCommand
            {
                Email = loginDto.Email,
                Password = loginDto.Password
            };
            var token = await _mediator.Send(command);
            return Ok(new { token = token });
        }
    }
}
