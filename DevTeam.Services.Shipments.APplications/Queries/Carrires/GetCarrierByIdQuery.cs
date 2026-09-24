//using DevTeam.Application.Mappings;
//using DevTeam.DTOs;
//using DevTeam.Entities;
//using DevTeam.Infrastructure;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Logging;
//using System.Text.Json;

//namespace DevTeam.Application.Queries.Carrires
//{
//    public class GetCarrierByIdQuery : IRequest<CarrierDto>
//    {
//        public int Id { get; set; }
//    }

//    public class GetCarrierByIdHandler
//        : IRequestHandler<GetCarrierByIdQuery, CarrierDto>
//    {
//        private readonly IRepository<Carrier> _repository;
//        private readonly IDistributedCache _cache;
//        private readonly ILogger<GetCarrierByIdHandler> _logger;

//        public GetCarrierByIdHandler(
//            IRepository<Carrier> repository,
//            IDistributedCache cache,
//            ILogger<GetCarrierByIdHandler> logger)
//        {
//            _repository = repository;
//            _cache = cache;
//            _logger = logger;
//        }

//        public async Task<CarrierDto> Handle(
//            GetCarrierByIdQuery request,
//            CancellationToken cancellationToken)
//        {
//            // 1. Create Cache Key
//            var cacheKey = $"carrier:{request.Id}";

//            _logger.LogInformation("Checking Redis cache for Carrier {CarrierId} with key {CacheKey}",request.Id,cacheKey);

//            // 2. Try to get data from Redis
//            var cachedData = await _cache.GetStringAsync(cacheKey,cancellationToken);

//            // 3. Cache HIT
//            if (cachedData != null)
//            {
//                _logger.LogInformation(
//                    "Redis Cache HIT for Carrier {CarrierId}",
//                    request.Id);

//                return JsonSerializer.Deserialize<CarrierDto>(cachedData)!;
//            }

//            // 4. Cache MISS
//            _logger.LogInformation(
//                "Redis Cache MISS for Carrier {CarrierId}. Fetching from database.",
//                request.Id);

//            var carrier = await _repository
//                .Query()
//                .AsNoTracking()
//                .Where(x => x.Id == request.Id)
//                .Select(x => x.ToDto())
//                .FirstOrDefaultAsync(cancellationToken);

//            // 5. Carrier doesn't exist
//            if (carrier == null)
//            {
//                _logger.LogWarning(
//                    "Carrier {CarrierId} was not found in database.",
//                    request.Id);

//                return null;
//            }

//            // 6. Convert to JSON
//            var json = JsonSerializer.Serialize(carrier);

//            // 7. Store in Redis
//            await _cache.SetStringAsync(cacheKey,json,
//                new DistributedCacheEntryOptions
//                {
//                    AbsoluteExpirationRelativeToNow =
//                        TimeSpan.FromMinutes(10)
//                },
//                cancellationToken);

//            _logger.LogInformation(
//                "Carrier {CarrierId} was stored in Redis cache with TTL of 10 minutes.",
//                request.Id);

//            // 8. Return data
//            return carrier;
//        }
//    }
//}

using DevTeam.Services.Shipments.Applications.DTOs;
using DevTeam.Services.Shipments.Applications.Mappings;
using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Application.Queries.Carrires
{
    public class GetCarrierByIdQuery : IRequest<CarrierDto>
    {
        public int Id { get; set; }
    }

    public class GetCarrierByIdHandler
        : IRequestHandler<GetCarrierByIdQuery, CarrierDto>
    {
        private readonly IRepository<Carrier> _repository;
        private readonly ICache _cache;
        private readonly ILogger<GetCarrierByIdHandler> _logger;

        public GetCarrierByIdHandler(
            IRepository<Carrier> repository,
            ICache cache,
            ILogger<GetCarrierByIdHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CarrierDto> Handle(
            GetCarrierByIdQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Create Cache Key
            var cacheKey = $"carrier:{request.Id}";

            _logger.LogInformation(
                "Checking Redis cache for Carrier {CarrierId} with key {CacheKey}",
                request.Id,
                cacheKey);

            // 2. Try to get data from Redis
            var cachedCarrier = await _cache.GetAsync<CarrierDto>(cacheKey, cancellationToken);

            // 3. Cache HIT
            if (cachedCarrier != null)
            {
                _logger.LogInformation(
                    "Redis Cache HIT for Carrier {CarrierId}",
                    request.Id);

                return cachedCarrier;
            }

            // 4. Cache MISS
            _logger.LogInformation(
                "Redis Cache MISS for Carrier {CarrierId}. Fetching from database.",
                request.Id);

            // 5. Get Carrier from database
            var carrier = await _repository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(x => x.ToDto())
                .FirstOrDefaultAsync(cancellationToken);

            // 6. Carrier doesn't exist
            if (carrier == null)
            {
                _logger.LogWarning(
                    "Carrier {CarrierId} was not found in database.",
                    request.Id);

                return null;
            }

            // 7. Store Carrier in Redis
            await _cache.SetAsync(
                cacheKey,
                carrier,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            _logger.LogInformation(
                "Carrier {CarrierId} was stored in Redis cache with TTL of 10 minutes.",
                request.Id);

            // 8. Return data
            return carrier;
        }
    }
}