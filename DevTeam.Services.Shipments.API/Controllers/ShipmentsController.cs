using DevTeam.Application.Messaging.ShipmentMessages;
using DevTeam.Application.Queries.GetShipmentById;
using DevTeam.Application.Queries.GetShipments;
using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Messaging.Publisher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevTeam.Services.Shipments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly IRabbitMQPublisher _publisher;
    private readonly IMediator _mediator;

    public ShipmentsController(IRabbitMQPublisher publisher, IMediator mediator)
    {
        _publisher = publisher;
        _mediator = mediator;
    }


    // GET: api/shipments
    [HttpGet]
    public async Task<ActionResult<List<ShipmentDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetShipmentsQuery());

        return Ok(result);
    }


    // GET: api/shipments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShipmentDto>> GetById(
        int id)
    {
        var result = await _mediator.Send(
            new GetShipmentByIdQuery
            {
                Id = id
            });


        if (result == null)
            return NotFound();


        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateShipmentDto dto)
    {
        var message = new CreateShipmentMessage
        {
            TrackingNumber = dto.TrackingNumber,
            Origin = dto.Origin,
            Destination = dto.Destination,
            Weight = dto.Weight,
            Status = dto.Status,
            CarrierId = dto.CarrierId
        };

        await _publisher.PublishAsync("Shipment", message);

        return Accepted(new
        {
            message = "Shipment creation request queued successfully.",
            trackingNumber = dto.TrackingNumber
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
      int id,
      UpdateShipmentDto dto)
    {
        var message = new UpdateShipmentMessage
        {
            Id = id,

            TrackingNumber =
                dto.TrackingNumber,

            Origin =
                dto.Origin,

            Destination =
                dto.Destination,

            Weight =
                dto.Weight,

            Status =
                dto.Status,

            CarrierId =
                dto.CarrierId
        };

        await _publisher.PublishAsync("ShipmenT", message);

        return Accepted(new
        {
            message =
                "Shipment update request queued successfully.",

            shipmentId = id
        });


    }

    // DELETE: api/Shipments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var message =
            new DeleteShipmentMessage
            {
                Id = id
            };

        await _publisher.PublishAsync("Shipment", message);

        return Accepted(new
        {
            message =
                "Shipment deletion request queued successfully.",

            shipmentId = id
        });
    }
}
//public class ShipmentsController : ControllerBase
//{
//    private readonly IMediator _mediator;


//    public ShipmentsController(IMediator mediator)
//    {
//        _mediator = mediator;
//    }


//    // GET: api/shipments
//    [HttpGet]
//    public async Task<ActionResult<List<ShipmentDto>>> GetAll()
//    {
//        var result = await _mediator.Send(
//            new GetShipmentsQuery());

//        return Ok(result);
//    }


//    // GET: api/shipments/1
//    [HttpGet("{id:int}")]
//    public async Task<ActionResult<ShipmentDto>> GetById(
//        int id)
//    {
//        var result = await _mediator.Send(
//            new GetShipmentByIdQuery
//            {
//                Id = id
//            });


//        if (result == null)
//            return NotFound();


//        return Ok(result);
//    }


//    // POST: api/shipments
//    [HttpPost]
//    public async Task<ActionResult<ShipmentDto>> Create(
//        CreateShipmentDto dto)
//    {
//        var command = dto.ToCommand();


//        var result = await _mediator.Send(command);


//        return Ok(result);
//    }


//    // PUT: api/shipments/1
//    [HttpPut("{id:int}")]
//    public async Task<ActionResult<ShipmentDto>> Update(int id,
//        UpdateShipmentDto dto)
//    {
//        var command = dto.ToCommand(id);

//        var result = await _mediator.Send(command);
//        if (result == null)
//            return NotFound();


//        return Ok(result);
//    }


//    // DELETE: api/shipments/1
//    [HttpDelete("{id:int}")]
//    public async Task<IActionResult> Delete(
//        int id)
//    {
//        var result = await _mediator.Send(
//            new DeleteShipmentCommand
//            {
//                Id = id
//            });


//        if (!result)
//            return NotFound();


//        return NoContent();
//    }
//}