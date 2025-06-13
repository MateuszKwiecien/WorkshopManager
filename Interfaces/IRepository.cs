using System.Linq.Expressions;

namespace WorkshopManager.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();
        Task<T?>        GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveAsync();
        Task<T> GetAsync(int dtoPartId);
    }
}