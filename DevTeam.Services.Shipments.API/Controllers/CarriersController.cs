using DevTeam.Application.Queries.Carrires;
using DevTeam.Services.Shipments.Applications.Commands.Carriers;
using DevTeam.Services.Shipments.Applications.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevTeam.Services.Shipments.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarriersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarriersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/carriers
        [HttpGet]
        public async Task<ActionResult<List<CarrierDto>>> GetAll()
        {
            var result = await _mediator.Send(
                new GetCarriersQuery());
            return Ok(result);
        }


        // GET: api/carriers/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CarrierDto>> GetById(
            int id)
        {
            var result = await _mediator.Send(
                new GetCarrierByIdQuery
                {
                    Id = id
                });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CarrierDto>> Create(CreateCarrierDto dto)
        {
            var command = new CreateCarrierCommand
            {
                Name = dto.Name,
                Code = dto.Code,
                ServiceLevels = dto.ServiceLevels
            };

            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CarrierDto>> Update(
        int id,
        UpdateCarrierDto dto)
        {
            var command = new UpdateCarrierCommand
            {
                Id = id,
                Name = dto.Name,
                Code = dto.Code,
                ServiceLevels = dto.ServiceLevels,
                UpdateBy = dto.UpdateBy
            };

            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(
             new DeleteCarrierCommand
             {
                 Id = id
             });

            //if (!result)
            //    return NotFound();

            return NoContent();
        }

    }
}