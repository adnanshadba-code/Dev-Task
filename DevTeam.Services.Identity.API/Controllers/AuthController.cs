using DevTeam.Services.Identity.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevTeam.Services.Identity.Application.Mappings;
namespace DevTeam.Services.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var command = dto.ToCommand();

        var result = await _mediator.Send(command);

        return Ok(result);
    }
}