

using DevTeam.Application.Messaging.ShipmentEventMessage;
using DevTeam.Application.Queries.ShipmentsEvents;
using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Messaging.Publisher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevTeam.Services.Shipments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentEventsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRabbitMQPublisher _publisher;

    public ShipmentEventsController(
        IMediator mediator,
        IRabbitMQPublisher publisher)
    {
        _mediator = mediator;
        _publisher = publisher;
    }

    // GET: api/ShipmentEvents
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetShipmentEventsQuery());
        return Ok(result);
    }

    // GET: api/ShipmentEvents/18
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetShipmentEventByIdQuery
        {
            Id = id
        });

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // GET: api/ShipmentEvents/shipment/1
    [HttpGet("shipment/{shipmentId:int}")]
    public async Task<IActionResult> GetByShipment(int shipmentId)
    {
        try
        {
            var result = await _mediator.Send(new GetShipmentEventsByShipmentQuery
            {
                ShipmentId = shipmentId
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // POST: api/ShipmentEvents
    [HttpPost]
    //[Authorize(Policy = "AddEvent")]
    public async Task<IActionResult> Create(CreateShipmentEventDto dto)
    {

        //        try
        //        {
        //            var result = await _mediator.Send(
        //                new GetShipmentEventsByShipmentQuery
        //                {
        //                    ShipmentId = shipmentId
        //                });

        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return NotFound(ex.Message);
        //        }
        //}Command سيتم إنشاؤه لاحقاً داخل الـ Worker

        var message = new CreateShipmentEventMessage
        {
            ShipmentId = dto.ShipmentId,
            Status = dto.Status,
            //  CreateBy = dto.CreateBy,
            LocationId = dto.LocationId
        };

        await _publisher.PublishAsync("ShipmentEvent", message);

        return Accepted(new
        {
            message = "Shipment event creation request queued successfully.",

            shipmentId = dto.ShipmentId
        });
    }

    // PUT: api/ShipmentEvents/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateShipmentEventDto dto)
    {
        var message = new UpdateShipmentEventMessage
        {
            Id = id,
            Status = dto.Status,
            ShipmentId = dto.ShipmentId,
            LocationId = dto.LocationId,
            //  UpdateBy = dto.UpdateBy
        };

        await _publisher.PublishAsync("ShipmentEvent", message);

        return Accepted(new
        {
            message = "Shipment event update request queued successfully.",
            shipmentEventId = id
        });
    }

    // DELETE: api/ShipmentEvents/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var message = new DeleteShipmentEventMessage
        {
            Id = id
        };

        await _publisher.PublishAsync("ShipmentEvent", message);

        return Accepted(new
        {
            message = "Shipment event deletion request queued successfully.",
            shipmentEventId = id
        });
    }
}