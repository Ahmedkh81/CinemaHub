using System.Linq.Expressions;

namespace CinemaHub.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null
            , Expression<Func<T, object>>[]? includes = null, bool tracked = true);
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null
            , Expression<Func<T, object>>[]? includes = null, bool tracked = true);
        void RemoveRange(IEnumerable<T> entities);
    }
}
