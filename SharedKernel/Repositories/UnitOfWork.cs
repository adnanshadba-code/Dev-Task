


//using SharedKernel.Repositories;

//public class UnitOfWork : IUnitOfWork
//{
//    private readonly AppDbContext _context;

//    public UnitOfWork(AppDbContext context)
//    {
//        _context = context;
//    }

//    public AppDbContext Context => _context;
//    public async Task<int> SaveChangesAsync()
//    {
//        return await _context.SaveChangesAsync();
//    }

//}

using Microsoft.EntityFrameworkCore;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _context;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public TContext Context => _context;


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}