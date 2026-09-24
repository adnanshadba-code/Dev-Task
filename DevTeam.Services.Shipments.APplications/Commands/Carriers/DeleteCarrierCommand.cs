using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Carriers
{
    public class DeleteCarrierCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteCarrierCommandHandler : IRequestHandler<DeleteCarrierCommand, bool>
    {
        private readonly IRepository<Carrier> _repository;
        private readonly ICache _cache;
        private readonly ILogger<DeleteCarrierCommandHandler> _logger;

        public DeleteCarrierCommandHandler(
            IRepository<Carrier> repository,
            ICache cache,
            ILogger<DeleteCarrierCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCarrierCommand request, CancellationToken cancellationToken)
        {
            var carrier = await _repository
                .Query()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (carrier == null)
            {
                return false;
            }

            _repository.Delete(carrier);

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Carrier {CarrierId} deleted successfully from database.",
                request.Id);

            // Remove single carrier cache
            await _cache.RemoveAsync(
                $"carrier:{request.Id}",
                cancellationToken);

            // Remove all carriers cache
            await _cache.RemoveAsync(
                "carriers:all",
                cancellationToken);

            _logger.LogInformation(
                "Redis cache invalidated for Carrier {CarrierId}.",
                request.Id);

            return true;
        }
    }
}