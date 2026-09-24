//namespace SharedKernel.Repositories
//{
//    public interface IRepository<T> where T : class
//    {

//        Task<List<T>> GetAllAsync();//Task<List<Shipment>> GetAllAsync(); / Task<List<Location>> GetAll();

//        Task<T?> GetByIdAsync(int id); // Task<Shipment> GetById(inr id); 

//        Task AddAsync(T entity);  // Task Add(Shipment sh);

//        void Update(T entity);

//        void Delete(T entity);

//        IQueryable<T> Query();

//        Task<int> SaveChangesAsync();
//    }
//}
namespace SharedKernel.Repositories;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    IQueryable<T> Query();

    Task<int> SaveChangesAsync();
}