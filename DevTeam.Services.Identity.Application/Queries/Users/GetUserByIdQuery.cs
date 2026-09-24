using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Caching.Distributed;  // to use  Idistributed 
using SharedKernel.Repositories;
using System.Text.Json; // to transfer betweenJson and object 

namespace DevTeam.Services.Identity.Application.Queries.Users
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public int Id { get; set; }
    }


    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IRepository<User> _repository;
        private readonly IDistributedCache _distributedCache;

        public GetUserByIdHandler(IRepository<User> repository, IDistributedCache distributedCache)
        {
            _repository = repository;
            _distributedCache = distributedCache;
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {

            //1-create Cache key 

            //example => 
            //user:5  → UserDto
            //user: 20 → UserDto

            var cacheKey = $"user:{request.Id}";

            //2-Check redis
            var cachedUser = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (cachedUser != null)
            {
                //cache hit 
                return JsonSerializer.Deserialize<UserDto>(cachedUser);

            }

            //3 cache Miss and go to repository to retreive data from database 

            var user = await _repository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (user == null)
                return null;

            //Entity to Dto => to response to client
            var userDto = user.ToDto();

            //convert DTO  to Json 
            var json = JsonSerializer.Serialize(userDto);

            //save data in redis 
            var cacheOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };


            await _distributedCache.SetStringAsync(cacheKey, json, cacheOption, cancellationToken);


            // return data to client 
            return userDto;

        }
    }

}
