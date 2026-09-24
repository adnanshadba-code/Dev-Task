using DevTeam.Services.Shipments.Applications.Commands.Shipments;
using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Domain.Entities;

namespace DevTeam.Services.Shipments.Applications.Mappings;

public static class ShipmentMapping
{
    // CreateShipmentDto → CreateShipmentCommand
    public static CreateShipmentCommand ToCommand(
        this CreateShipmentDto dto)
    {
        return new CreateShipmentCommand
        {
            TrackingNumber = dto.TrackingNumber,
            Origin = dto.Origin,
            Destination = dto.Destination,
            Weight = dto.Weight,
            Status = dto.Status,
            CarrierId = dto.CarrierId
        };
    }


    // UpdateShipmentDto → UpdateShipmentCommand
    public static UpdateShipmentCommand ToCommand(
        this UpdateShipmentDto dto,
        int id)
    {
        return new UpdateShipmentCommand
        {
            Id = id,
            TrackingNumber = dto.TrackingNumber,
            Origin = dto.Origin,
            Destination = dto.Destination,
            Weight = dto.Weight,
            Status = dto.Status,
            CarrierId = dto.CarrierId,
            UpdateBy = dto.UpdateBy
        };
    }


    // CreateShipmentCommand → Shipment
    public static Shipment ToEntity(
        this CreateShipmentCommand command)
    {
        return new Shipment
        {
            TrackingNumber = command.TrackingNumber,
            Origin = command.Origin,
            Destination = command.Destination,
            Weight = command.Weight,
            Status = command.Status,
            CarrierId = command.CarrierId,

            CreatedAt = DateTime.UtcNow,
            // UpdatedAt = DateTime.UtcNow
        };
    }


    // Shipment → ShipmentDto
    public static ShipmentDto ToDto(
        this Shipment shipment)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,

            TrackingNumber = shipment.TrackingNumber,

            Origin = shipment.Origin,

            Destination = shipment.Destination,

            Weight = (decimal)shipment.Weight,

            Status = shipment.Status,

            CarrierId = shipment.CarrierId,

            CarrierName = shipment.Carrier?.Name,

            Events = shipment.ShipmentEvents?
                .Select(e => new ShipmentEventDto
                {
                    Id = e.Id,

                    Status = e.Status,

                    ShipmentId = e.ShipmentId,

                    LocationId = e.LocationId,

                    LocationName = e.Location?.Name
                })
                .ToList()
                ?? new List<ShipmentEventDto>()
        };
    }


    // UpdateShipmentCommand → Shipment
    public static Shipment UpdateFrom(
        this UpdateShipmentCommand command,
        Shipment shipment)
    {
        shipment.TrackingNumber = command.TrackingNumber;

        shipment.Origin = command.Origin;

        shipment.Destination = command.Destination;

        shipment.Weight = command.Weight;

        shipment.Status = command.Status;

        shipment.CarrierId = command.CarrierId;

        shipment.UpdateBy = command.UpdateBy;

        shipment.UpdatedAt = DateTime.UtcNow;

        return shipment;
    }
}