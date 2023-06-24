using MagicVilla_API.Model;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        public Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string? includeProperties = null);
        public Task CreateVillaAsync(T entity);
        public Task RemoveAsync(T entity);
        public Task SaveAsync();
    }
}
