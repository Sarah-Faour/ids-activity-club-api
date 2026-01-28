using System.Linq.Expressions;

namespace ActivityClub.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
        Task SaveChangesAsync();

        Task LoadReferenceAsync<TProperty>(T entity, Expression<Func<T, TProperty>> reference)
            where TProperty : class;

        Task LoadCollectionAsync<TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> collection)
            where TProperty : class;

    }
}

