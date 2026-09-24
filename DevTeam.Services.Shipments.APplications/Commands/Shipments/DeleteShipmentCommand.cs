using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Shipments
{

    public class DeleteShipmentCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteShipmentHandler
    : IRequestHandler<DeleteShipmentCommand, bool>
    {
        private readonly IRepository<Shipment> _repository;



        public DeleteShipmentHandler(
            IRepository<Shipment> repository)
        {
            _repository = repository;
        }


        public async Task<bool> Handle(
            DeleteShipmentCommand request,
            CancellationToken cancellationToken)
        {
            var shipment = await _repository
                .GetByIdAsync(request.Id);


            if (shipment == null)
                throw new Exception(
                    $"Shipment with Id {request.Id} does not exist.");


            _repository.Delete(shipment);


            await _repository.SaveChangesAsync();


            return true;
        }
    }
}

