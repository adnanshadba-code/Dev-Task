using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Shipments
{

    public class CreateShipmentCommand : IRequest<ShipmentDto>
    {
        public string TrackingNumber { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public string Status { get; set; } = string.Empty;

        public int CarrierId { get; set; }
    }


    public class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, ShipmentDto>
    {
        private readonly IRepository<Shipment> _repository;
        private readonly IRepository<Carrier> _carrierRepository;

        public CreateShipmentHandler(IRepository<Shipment> repository,
            IRepository<Carrier> carrierRepository)
        {
            _repository = repository;
            _carrierRepository = carrierRepository;
        }

        public async Task<ShipmentDto> Handle(
            CreateShipmentCommand request,
            CancellationToken cancellationToken)
        {
            var carrierExists =
                await _carrierRepository
                    .Query()
                    .AnyAsync(
                        c => c.Id == request.CarrierId,
                        cancellationToken);

            if (!carrierExists)
                throw new Exception("Carrier does not exist.");

            var shipment = request.ToEntity();

            await _repository.AddAsync(shipment);

            await _repository.SaveChangesAsync();

            return shipment.ToDto();
        }
    }

    //public class CreateShipmentHandler
    //  : IRequestHandler<CreateShipmentCommand, ShipmentDto>
    //{
    //    private readonly IRepository<Shipment> _repository;

    //    private readonly IRepository<Carrier> _carrierRepository;
    //    private readonly IRabbitMQPublisher _rabbitMQPublisher;


    //    public CreateShipmentHandler(
    //        IRepository<Shipment> repository,
    //        IRepository<Carrier> carrierRepository,
    //         IRabbitMQPublisher publisher)
    //    {
    //        _repository = repository;
    //        _carrierRepository = carrierRepository;
    //        _rabbitMQPublisher= publisher;
    //    }


    //    public async Task<ShipmentDto> Handle(
    //        CreateShipmentCommand request,
    //        CancellationToken cancellationToken)
    //    {
    //        var carrierExists = await _carrierRepository.Query().AnyAsync(
    //                c => c.Id == request.CarrierId,
    //                cancellationToken);

    //        if (!carrierExists)
    //            throw new Exception("Carrier does not exist.");


    //        var shipment = request.ToEntity();


    //        await _repository.AddAsync(shipment);


    //        await _repository.SaveChangesAsync();

    //        // 3️⃣ Create RabbitMQ message
    //        var message = new ShipmentCreatedMessage
    //        {
    //            ShipmentId = shipment.Id,
    //            TrackingNumber = shipment.TrackingNumber,
    //            Origin = shipment.Origin,
    //            Destination = shipment.Destination,
    //            Weight = (decimal)shipment.Weight,
    //            CarrierId = shipment.CarrierId,
    //            Status = shipment.Status
    //        };

    //        // 4️⃣ Send message to RabbitMQ
    //        await _rabbitMQPublisher.PublishAsync(message);


    //        return shipment.ToDto();
    //    }
    //}
}
