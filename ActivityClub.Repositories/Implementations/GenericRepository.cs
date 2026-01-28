using ActivityClub.Data.Models; // adjust if your namespace differs
using ActivityClub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ActivityClub.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ActivityClubDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ActivityClubDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Remove(T entity)
            => _dbSet.Remove(entity);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);

        public IQueryable<T> Query()
            => _dbSet.AsQueryable();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task LoadReferenceAsync<TProperty>(T entity, Expression<Func<T, TProperty>> reference)
    where TProperty : class
        {
            await _context.Entry(entity).Reference(reference).LoadAsync();
        }

        public async Task LoadCollectionAsync<TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> collection)
            where TProperty : class
        {
            await _context.Entry(entity).Collection(collection).LoadAsync();
        }

    }
}

