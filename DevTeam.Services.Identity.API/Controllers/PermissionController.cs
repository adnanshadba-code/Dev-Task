using DevTeam.Services.Identity.Application.Commands.Permissions;
using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Application.Queries.Permissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevTeam.Services.Identity.Application.Mappings;
using Microsoft.AspNetCore.Authorization;

namespace DevTeam.Services.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // =========================================
    // GET: api/Permissions
    // =========================================

    [HttpGet]
    public async Task<ActionResult<List<PermissionDto>>> GetAll()
    {
        var result = await _mediator.Send(
            new GetPermissionsQuery());

        return Ok(result);
    }


    // =========================================
    // GET: api/Permissions/1
    // =========================================

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PermissionDto>> GetById(
        int id)
    {
        var result = await _mediator.Send(
            new GetPermissionByIdQuery
            {
                Id = id
            });

        if (result == null)
            return NotFound("Permission not found.");

        return Ok(result);
    }


    // =========================================
    // POST: api/Permissions
    // =========================================

    [HttpPost]
    [Authorize("Create")]
    public async Task<ActionResult<PermissionDto>> Create(CreatePermissionDto dto)
    {
        var command = dto.ToCommand();

        var result = await _mediator.Send(command);

        return Ok(result);
    }


    // =========================================
    // PUT: api/Permissions/1
    // =========================================

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PermissionDto>> Update(int id, UpdatePermissionDto dto)
    {
        var command = dto.ToCommand(id);

        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound("Permission not found.");

        return Ok(result);
    }


    // =========================================
    // DELETE: api/Permissions/1
    // =========================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var result = await _mediator.Send(
            new DeletePermissionCommand
            {
                Id = id
            });

        if (!result)
            return NotFound("Permission not found.");

        return NoContent();
    }
}