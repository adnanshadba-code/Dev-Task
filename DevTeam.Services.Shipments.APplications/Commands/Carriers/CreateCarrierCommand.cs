//using DevTeam.Application.Mappings;
//using DevTeam.DTOs;
//using DevTeam.Infrastructure;
//using DevTeam.Infrastructure.Repositories;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace DevTeam.Application.Commands.Carriers
//{
//    public class CreateCarrierCommand : IRequest<CarrierDto>
//    {
//        public string Name { get; set; } = string.Empty;
//        public string Code { get; set; } = string.Empty;
//        public string ServiceLevels { get; set; }
//    }

//    public class CreateCarrierHandler : IRequestHandler<CreateCarrierCommand, CarrierDto>
//    {
//        private readonly IRepository<Carrier> _repository;
//        private readonly IUnitOfWork _unitOfWork;

//        public CreateCarrierHandler(IRepository<Carrier> repository , IUnitOfWork unitOfWork)
//        {
//            _repository = repository;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<CarrierDto> Handle(CreateCarrierCommand request, CancellationToken cancellationToken)
//        {
//            var carrier = request.ToEntity();

//            await _repository.AddAsync(carrier);

//            await _unitOfWork.SaveChangesAsync();

//            return carrier.ToDto();
//        }
//    }
//}
using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Carriers
{
    public class CreateCarrierCommand : IRequest<CarrierDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string ServiceLevels { get; set; } = string.Empty;
    }

    public class CreateCarrierHandler
        : IRequestHandler<CreateCarrierCommand, CarrierDto>
    {
        private readonly IRepository<Carrier> _repository;
        private readonly ICache _cache;
        private readonly ILogger<CreateCarrierHandler> _logger;

        public CreateCarrierHandler(IRepository<Carrier> repository, ICache cache, ILogger<CreateCarrierHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CarrierDto> Handle(
            CreateCarrierCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Convert Command -> Entity
            var carrier = request.ToEntity();

            // 2. Add entity to Repository
            await _repository.AddAsync(carrier);

            // 3. Save changes to PostgreSQL
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Carrier {CarrierId} created successfully in database.", carrier.Id);

            // 4. Invalidate cached list
            await _cache.RemoveAsync("carriers:all", cancellationToken);

            _logger.LogInformation("Redis cache invalidated for all carriers.");

            // 5. Return Entity -> DTO
            return carrier.ToDto();
        }
    }
}