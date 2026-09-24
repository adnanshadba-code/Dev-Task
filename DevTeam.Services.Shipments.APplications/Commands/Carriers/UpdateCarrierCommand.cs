using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Carriers
{

    //? if Carrier not found
    // when  comand excute hanlder => return CarrierDto 
    // response or output 
    public class UpdateCarrierCommand : IRequest<CarrierDto?>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string ServiceLevels { get; set; } = string.Empty;
        public string? UpdateBy { get; set; }
        public string UpdateAt { get; set; } = string.Empty;
    }

    public class UpdateCarrierCommandHandler
        : IRequestHandler<UpdateCarrierCommand, CarrierDto?>
    {
        private readonly IRepository<Carrier> _repository;
        private readonly ICache _cache;
        private readonly ILogger<UpdateCarrierCommandHandler> _logger;

        public UpdateCarrierCommandHandler(
            IRepository<Carrier> repository,
            ICache cache,
            ILogger<UpdateCarrierCommandHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CarrierDto?> Handle(
            UpdateCarrierCommand request,
            CancellationToken cancellationToken)
        {
            var carrier = await _repository
                .Query()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);

            if (carrier == null)
            {
                return null;
            }

            carrier = request.UpdateFrom(carrier);

            _repository.Update(carrier);

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Carrier {CarrierId} updated successfully in database.",
                request.Id);

            // Remove single carrier cache
            await _cache.RemoveAsync($"carrier:{request.Id}", cancellationToken);

            // Remove all carriers cache
            await _cache.RemoveAsync("carriers:all", cancellationToken);

            _logger.LogInformation(
                "Redis cache invalidated for Carrier {CarrierId}.",
                request.Id);

            return carrier.ToDto();
        }
    }
}