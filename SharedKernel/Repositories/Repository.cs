//using Microsoft.EntityFrameworkCore;

//namespace SharedKernel.Repositories;

//public class Repository<T> : IRepository<T>where T : class
//{
//    private readonly IUnitOfWork _unitOfWork;
//    //private readonly IRepository<T> _repository;

//    private readonly DbSet<T> _dbSet; // any table

//    public Repository(IUnitOfWork unitOfWork)
//    {
//        _unitOfWork = unitOfWork;

//        _dbSet = _unitOfWork.Context.Set<T>();
//    }

//    public async Task<List<T>> GetAllAsync()
//    {
//        return await _dbSet.ToListAsync();
//    }

//    public async Task<T?> GetByIdAsync(int id)
//    {
//        return await _dbSet.FindAsync(id);
//    }

//    public async Task AddAsync(T entity)
//    {
//        await _dbSet.AddAsync(entity);
//    }

//    public void Update(T entity)
//    {
//        _dbSet.Update(entity);
//    }

//    public void Delete(T entity)
//    {
//        _dbSet.Remove(entity);
//    }

//    public IQueryable<T> Query()
//    {
//        return _dbSet;
//    }

//    public async Task<int> SaveChangesAsync()
//    {
//        return await _unitOfWork.SaveChangesAsync();
//    }

//}

using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Repositories;

public class Repository<T, TContext> : IRepository<T>
    where T : class
    where TContext : DbContext
{
    private readonly IUnitOfWork<TContext> _unitOfWork;
    private readonly DbSet<T> _dbSet;

    public Repository(IUnitOfWork<TContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _dbSet = _unitOfWork.Context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public IQueryable<T> Query()
    {
        return _dbSet;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _unitOfWork.SaveChangesAsync();
    }
}