using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.ShipmentEvents
{
    public class DeleteShipmentEventCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteShipmentEventHanmdler : IRequestHandler<DeleteShipmentEventCommand, bool>
    {

        private readonly IRepository<ShipmentEvent> _repository;

        public DeleteShipmentEventHanmdler(IRepository<ShipmentEvent> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteShipmentEventCommand request, CancellationToken cancellationToken)
        {
            var resault = await _repository.Query().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (resault == null)
            {
                return false;
            }
            _repository.Delete(resault);
            return true;
        }
    }
}
