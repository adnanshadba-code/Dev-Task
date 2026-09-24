using DevTeam.Services.Identity.Application.Commands.Users;
using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevTeam.Services.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // GET: api/Users
    // =========================================

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var result = await _mediator.Send(
            new GetUsersQuery());

        return Ok(result);
    }

    // GET: api/Users/5
    // =========================================

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery
        {
            Id = id
        });

        if (result == null)
            return NotFound();

        return Ok(result);
    }


    // POST: api/Users

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        var command = dto.ToCommand();

        var result = await _mediator.Send(command);

        return Ok(result);
    }


    // PUT: api/Users/5

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id,UpdateUserDto dto)
    {
        var command = dto.ToCommand(id);

        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }


    // DELETE: api/Users/5

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(
            new DeleteUserCommand
            {
                Id = id
            });

        if (!result)
            return NotFound();

        return NoContent();
    }
}