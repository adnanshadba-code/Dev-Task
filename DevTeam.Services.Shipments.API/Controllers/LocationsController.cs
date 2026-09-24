using DevTeam.Application.Queries.Locations;
using DevTeam.Services.Shipments.Applications.Commands.Locations;
using DevTeam.Services.Shipments.Applications.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevTeam.Services.Shipments.Applications.Mappings;

namespace DevTeam.Services.Shipments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<LocationDto>>> GetAll()
    {
        var result = await _mediator.Send(
            new GetLocationsQuery());//send requedt using MeditR to GetLocationsQuery // هذا طلبي 

        return Ok(result);//200
        //return NotFound() 400
    }

    // GET: api/Locations/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocationDto>> GetById(
        int id)
    {
        //MediatR عنده طلب 
        // GetLocationsQuery اسمه 
        var result = await _mediator.Send(
            new GetLocationByIdQuery  //Request
            {
                Id = id // 
            });

        if (result == null)
            return NotFound();

        return Ok(result);
    }
    //}

    // POST: api/locations
    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Create(CreateLocationDto dto)
    {
        //Dto => Command || Mapping
        var command = dto.ToCommand();
        //send Command => Handler 
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    //ToCommand Overload method (same name ,  deffirent parameter)(

    [HttpPut("{id:int}")]
    public async Task<ActionResult<LocationDto>> Update(
       int id,UpdateLocationDto dto)
    {
        // DTO → Command
        var command = dto.ToCommand(id);

        // Send Command
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }



    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var result = await _mediator.Send(
            new DeleteLocationCommand
            {
                Id = id
            });

        //if (!result)
        //    return NotFound();

        return NoContent();
    }










}