using DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents;
using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Domain.Entities;

namespace DevTeam.Services.Shipments.Applications.Mappings
{

    // static => not need to new instance (obJ)  , you can get accsesss method using Class direct not usnig object   (ShipmentEventMapping.  ...)
    public static class ShipmentEventMapping
    {


        // Create 

        //2 mapping recived from client 
        //this => exctention method 
        public static CreateShipmentEventCommand ToCommand(this CreateShipmentEventDto createShipmentEventDto)
        {
            return new CreateShipmentEventCommand
            {
                CreateBy = createShipmentEventDto.CreateBy,
                LocationId = createShipmentEventDto.LocationId,
                ShipmentId = createShipmentEventDto.ShipmentId,
                Status = createShipmentEventDto.Status,
            };
        }


        /**************************************************************************************/
        //Command to Entity 
        public static ShipmentEvent ToEntity(this CreateShipmentEventCommand command)
        {
            return new ShipmentEvent()
            {
                Status = command.Status,
                ShipmentId = command.ShipmentId,
                LocationId = command.LocationId,
                CreateBy = command.CreateBy,
                CreatedAt = DateTime.UtcNow,
            };
        }

        //Entitiy to Dto
        public static ShipmentEventDto ToDto(this ShipmentEvent entity)
        {
            return new ShipmentEventDto()
            {
                Id = entity.Id,
                Status = entity.Status,
                ShipmentId = entity.ShipmentId,
                LocationId = entity.LocationId,
                LocationName = entity.Location?.Name
            };
        }

        ///////////////////////////////////////////////////
        // Update 

        //Request from client
        //يعني هي Mapping/Updating in-memory object.
        public static UpdateShipmentEventCommand ToCommand(this UpdateShipmentEventDto dto, int id)
        {
            return new UpdateShipmentEventCommand()
            {
                Id = id,
                Status = dto.Status,
                ShipmentId = dto.ShipmentId,
                LocationId = dto.LocationId,
                UpdateBy = dto.UpdateBy
            };
        }

        //Return response to client
        public static ShipmentEvent UpdateFrom(this UpdateShipmentEventCommand command, ShipmentEvent entity)
        {
            {
                entity.Status = command.Status;
                entity.ShipmentId = command.ShipmentId;
                entity.LocationId = command.LocationId;
                entity.UpdateBy = command.UpdateBy;
                entity.UpdatedAt = DateTime.UtcNow;

                return entity;
            }
        }


    }
}
