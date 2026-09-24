using DevTeam.Services.Shipments.Applications.Commands.Carriers;
using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Domain.Entities;

namespace DevTeam.Services.Shipments.Applications.Mappings
{

    //static class => no need to create object 
    public static class CarrierMapping
    {

        //    // Extension Method => sample method || Function  (function => Class  not object) 
        //    // CarrierDto => External , rertieved , return type
        //    // Carrier => internal 
        //    // this => Exctention method  
        //    //CarrierMapping.ToDto(carrier);  => carrier.ToDto();
        public static CarrierDto ToDto(this Carrier carrier)
        {
            return new CarrierDto
            {
                Id = carrier.Id,
                Code = carrier.Code,
                Name = carrier.Name,
                ServiceLevels = carrier.ServiceLevels
            };
        }

        public static Carrier ToEntity(this CreateCarrierCommand command)
        {
            return new Carrier
            {
                Name = command.Name,
                Code = command.Code,
                ServiceLevels = command.ServiceLevels
            };
        }

        public static Carrier UpdateFrom(this UpdateCarrierCommand command, Carrier carrier)
        {
            carrier.Name = command.Name;
            carrier.Code = command.Code;
            carrier.ServiceLevels = command.ServiceLevels;
            carrier.UpdateBy = command.UpdateBy;
            carrier.UpdatedAt = DateTime.UtcNow;

            return carrier;
        }


    }
}
