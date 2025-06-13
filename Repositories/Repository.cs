using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorkshopManager.Data;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly WorkshopDbContext _db;
        private readonly DbSet<T>          _set;

        public Repository(WorkshopDbContext db)
        {
            _db  = db;
            _set = db.Set<T>();
        }
        
        public IQueryable<T> Query() => _set.AsQueryable();
        public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);

        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? p = null) =>
            p is null ? await _set.ToListAsync()
                : await _set.Where(p).ToListAsync();

        public async Task AddAsync(T e) { await _set.AddAsync(e); }
        public void  Update(T e) => _set.Update(e);
        public void  Delete(T e) => _set.Remove(e);
        public Task<int> SaveAsync() => _db.SaveChangesAsync();
    }
}