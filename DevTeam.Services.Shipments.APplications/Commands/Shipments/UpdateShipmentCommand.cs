using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Shipments
{

    public class UpdateShipmentCommand : IRequest<ShipmentDto?>
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public string Status { get; set; } = string.Empty;

        public int CarrierId { get; set; }

        public string? UpdateBy { get; set; }
    }

    public class UpdateShipmentHandler : IRequestHandler<UpdateShipmentCommand, ShipmentDto?>
    {
        private readonly IRepository<Shipment> _repository;

        private readonly IRepository<Carrier> _carrierRepository;

        //private readonly IUnitOfWork _unitOfWork;


        public UpdateShipmentHandler(
            IRepository<Shipment> repository,
            IRepository<Carrier> carrierRepository
            //IUnitOfWork unitOfWork
            )
        {
            _repository = repository;
            _carrierRepository = carrierRepository;
            //_unitOfWork = unitOfWork;
        }


        public async Task<ShipmentDto?> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
        {
            // IQueryable < Shipment >

            var shipment = await _repository.Query().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (shipment == null)
                return null;


            var carrierExists = await _carrierRepository.Query().AnyAsync(x => x.Id == request.CarrierId, cancellationToken);


            if (!carrierExists)
                throw new Exception("Carrier does not exist.");


            request.UpdateFrom(shipment);


            _repository.Update(shipment);


            await _repository.SaveChangesAsync();


            return shipment.ToDto();
        }
    }
}