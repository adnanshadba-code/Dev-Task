

using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;

namespace DevTeam.Services.Identity.Application.Queries.Users
{
    public class GetUsersQuery : IRequest<List<UserDto>>
    {
    }
    public class GetUsersHandler: IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly IRepository<User> _repository;
        private readonly ICache _cache;


        public GetUsersHandler(IRepository<User> repository, ICache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "users:all";

            // 1. Check Redis
            //var cachedUsers = await _cache.GetStringAsync(cacheKey,cancellationToken);

            var cachedUsers = await _cache.GetAsync<List<UserDto>>(cacheKey, cancellationToken);

            //// 2. Cache HIT
            //if (cachedUsers != null)
            //{
            //    return JsonSerializer.Deserialize<List<UserDto>>(cachedUsers) ?? new List<UserDto>();
            //}
            if (cachedUsers != null)
            {
                return cachedUsers;
            }


            // 3. Cache MISS → Get from Database
            var users = await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);

            var userDtos = users.Select(x => x.ToDto()).ToList();

            await _cache.SetAsync(cacheKey, userDtos, TimeSpan.FromMinutes(10), cancellationToken);

            // 4. Convert DTOs → JSON
            //var json = JsonSerializer.Serialize(userDtos);

            //// 5. Cache for 10 minutes
            //var cacheOptions = new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow =
            //        TimeSpan.FromMinutes(10)
            //};


            // 6. Save in Redis
            //await _cache.SetStringAsync(
            //    cacheKey,
            //    json,
            //    cacheOptions,
            //    cancellationToken);

            // 7. Return data
            return userDtos;
        }
    }
}
