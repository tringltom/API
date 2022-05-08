using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly DataContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<TEntity> GetAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.SingleOrDefaultAsync(predicate);

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
        public async Task<IEnumerable<TEntity>> FindAsync(int? limit, int? offset, Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, int>> orderPredicate)
        {
            return await _dbSet
                .Where(wherePredicate)
                .OrderByDescending(orderPredicate)
                .Skip(offset ?? 0) // remove magic numbers
                .Take(limit ?? 3)
                .ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.AnyAsync(predicate);
        public async Task<int> CountAsync() => await _dbSet.CountAsync();
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.CountAsync(predicate);

        public void Add(TEntity entity) => _dbSet.Add(entity);
        public void AddRange(IEnumerable<TEntity> entities) => _dbSet.AddRange();

        public void Remove(TEntity entity) => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
    }
}
