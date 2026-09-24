using Microsoft.EntityFrameworkCore;

public interface IUnitOfWork<TContext> where TContext : DbContext
{
    TContext Context { get; }

    Task<int> SaveChangesAsync();
}