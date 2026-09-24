using DevTeam.Services.Shipments.Applications.Commands.Locations;
using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Domain.Entities;

namespace DevTeam.Services.Shipments.Applications.Mappings;

public static class LocationMapping
{

    // CreationDto to Command 
    //in handller => var command=dto.ToCommand()
    public static CreateLocationCommand ToCommand(this CreateLocationDto dto)
    {
        return new CreateLocationCommand()
        {
            Name = dto.Name,
            Longitude = dto.Longitude,
            Code = dto.Code,
            Country = dto.Country,
            Latitude = dto.Latitude
        };
    }

    // Command to Entity to save in DB 
    //var location =  ..  .ToEntity()
    public static Location ToEntity(this CreateLocationCommand command)
    {
        return new Location()
        {
            Code = command.Code,
            Name = command.Name,
            Country = command.Country,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }


    //Entity to Dto 
    //return Data to Client  
    public static LocationDto ToDto(this Location location)
    {
        return new LocationDto()
        {
            //Id= location.Id,
            Code = location.Code,
            Country = location.Country,
            Name = location.Name,
            Longitude = location.Longitude,
            Latitude = location.Latitude
        };
    }

    // =========================
    // Update DTO → Update Command
    // =========================

    public static UpdateLocationCommand ToCommand(
        this UpdateLocationDto dto,
        int id)
    {
        return new UpdateLocationCommand
        {
            Id = id,
            Code = dto.Code,
            Name = dto.Name,
            Country = dto.Country,
            Latitude = (decimal)dto.Latitude,
            Longitude = (decimal)dto.Longitude,
            UpdateBy = dto.UpdateBy
        };
    }

    // Update Command → Existing Entity
    // =========================

    public static Location UpdateFrom(
        this UpdateLocationCommand command,
        Location location)
    {
        location.Code = command.Code;
        location.Name = command.Name;
        location.Country = command.Country;
        location.Latitude = command.Latitude;
        location.Longitude = command.Longitude;
        location.UpdateBy = command.UpdateBy;
        location.UpdatedAt = DateTime.UtcNow;

        return location;
    }


}