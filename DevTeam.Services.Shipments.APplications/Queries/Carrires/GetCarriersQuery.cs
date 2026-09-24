//using DevTeam.Application.Mappings;
//using DevTeam.DTOs;
//using DevTeam.Infrastructure;
//using DevTeam.Infrastructure.Repositories;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Logging;
//using System.Text.Json;

//namespace DevTeam.Application.Queries.Carrires
//{
//    // IRequest => Interface in MediatR
//    public class GetCarriersQuery : IRequest<List<CarrierDto>>
//    {
//    }

//    // IRequestHandler => MediatR
//    public class GetCarriersHandler: IRequestHandler<GetCarriersQuery, List<CarrierDto>>
//    {
//        private readonly IRepository<Carrier> _repository;
//        private readonly IDistributedCache _cache;
//        private readonly ILogger<GetCarriersHandler> _logger;

//        public GetCarriersHandler(IRepository<Carrier> repository,IDistributedCache cache,ILogger<GetCarriersHandler> logger)
//        {
//            _repository = repository;
//            _cache = cache;
//            _logger = logger;
//        }


//        //var carriers =await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);
//        //            //return carriers.Select(x=>x.ToDto()).ToList();
//        //            // _repository.Query =>function in repositoery 
//        //            //AsNoTracking no need to use Change Tracker => change tracker using to save any changes 
//        //            // used AsNoTracking by Get to reduce Cpu , memory (Cost) => improve performance
//        //            //  used ASNoTracking on get Action (GetAll , GetById)
//        //            var carriers=await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);
//        public async Task<List<CarrierDto>> Handle(GetCarriersQuery request,CancellationToken cancellationToken)
//        {
//            // 1. Cache Key
//            const string cacheKey = "carriers:all";

//            _logger.LogInformation("Checking Redis cache for all carriers. Key: {CacheKey}",cacheKey);

//            // 2. Try to get data from Redis
//            var cachedData = await _cache.GetStringAsync(cacheKey,cancellationToken);

//            // 3. Cache HIT
//            if (cachedData != null)
//            {
//                _logger.LogInformation(
//                    "Redis Cache HIT for all carriers.");

//                return JsonSerializer.Deserialize<List<CarrierDto>>(
//                    cachedData)!;
//            }

//            // 4. Cache MISS
//            _logger.LogInformation(
//                "Redis Cache MISS for all carriers. Fetching from database.");

//            // 5. Get data from database
//            var carriers = await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);

//            var result = carriers.Select(x => x.ToDto()).ToList();

//            // 6. Convert result to JSON
//            var json = JsonSerializer.Serialize(result);

//            // 7. Store in Redis
//            await _cache.SetStringAsync(cacheKey,json,
//                new DistributedCacheEntryOptions
//                {
//                    AbsoluteExpirationRelativeToNow =TimeSpan.FromMinutes(10)
//                },
//                cancellationToken);

//            _logger.LogInformation("All carriers were stored in Redis cache with TTL of 10 minutes.");

//            // 8. Return result
//            return result;
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
    // IRequest => Interface in MediatR
    public class GetCarriersQuery : IRequest<List<CarrierDto>>
    {
    }

    // IRequestHandler => MediatR
    public class GetCarriersHandler
        : IRequestHandler<GetCarriersQuery, List<CarrierDto>>
    {
        private readonly IRepository<Carrier> _repository;
        private readonly ICache _cache;
        private readonly ILogger<GetCarriersHandler> _logger;

        public GetCarriersHandler(IRepository<Carrier> repository, ICache cache, ILogger<GetCarriersHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<CarrierDto>> Handle(
            GetCarriersQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Cache Key
            const string cacheKey = "carriers:all";

            _logger.LogInformation("Checking Redis cache for all carriers. Key: {CacheKey}", cacheKey);

            // 2. Try to get data from Redis
            var cachedCarriers = await _cache.GetAsync<List<CarrierDto>>(cacheKey, cancellationToken);

            // 3. Cache HIT
            if (cachedCarriers != null)
            {
                _logger.LogInformation("Redis Cache HIT for all carriers.");
                return cachedCarriers;
            }

            // 4. Cache MISS
            _logger.LogInformation("Redis Cache MISS for all carriers. Fetching from database.");

            // 5. Get data from database
            var carriers = await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);

            var result = carriers.Select(x => x.ToDto()).ToList();

            // 6. Store result in Redis
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10), cancellationToken);

            _logger.LogInformation("All carriers were stored in Redis cache with TTL of 10 minutes.");

            // 7. Return result
            return result;
        }
    }
}