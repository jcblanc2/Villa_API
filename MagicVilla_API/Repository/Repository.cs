using MagicVilla_API.Model;
using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MagicVilla_API.Repository.IRepository;

namespace MagicVilla_API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        internal DbSet<T> _DbSet;
        public Repository(ApplicationDbContext context)
        {
            _dbContext = context;
            _dbContext.VillasNumber.Include(u => u.Villa).ToList();
            this._DbSet = _dbContext.Set<T>();
        }

        public async Task CreateVillaAsync(T entity)
        {
            await _DbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, bool tracked, string? includeProperties = null)
        {
            IQueryable<T> query = _DbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var prop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = _DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var prop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }

            return await query.ToListAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _DbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}