using DevTeam.Services.Shipments.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Repositories;

namespace DevTeam.Services.Shipments.Applications.Commands.Locations
{
    public class DeleteLocationCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, bool>
    {

        private readonly IRepository<Location> _repository;

        public DeleteLocationHandler(IRepository<Location> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            var valDeleted = await _repository.Query().FirstOrDefaultAsync(z => z.Id == request.Id);
            // var valDeletaed= await _repo.Query().FindAsync(request.Id)
            if (valDeleted == null)
            {
                return false;
            }
            _repository.Delete(valDeleted);
            await _repository.SaveChangesAsync();  // Change  in DB
            return true;

        }
    }

}