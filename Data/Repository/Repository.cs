using System.Linq.Expressions;
using Data.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _entities;

        public Repository(DataContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _entities.Where(predicate).AsQueryable();

            return includes.Aggregate(query, (current, include) => current.Include(include));
        }

        public T? Delete(int id)
        {
            var entity = Find(id);
            if (entity is null)
            {
                return null;
            }

            _entities.Remove(entity);

            return entity;
        }

        public T Update(T entity)
        {
            return _entities.Update(entity).Entity;
        }

        public T? Find(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = _entities.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = _entities.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            return entities.Select(entity => _entities.Add(entity).Entity).ToList();
        }

        public IEnumerable<T> UpdateRange(IEnumerable<T> entities)
        {
            return entities.Select(entity => _entities.Update(entity).Entity).ToList();
        }

        public async Task<T> AddAsync(T entity)
        {
            var newEntity = await _entities.AddAsync(entity);

            return newEntity.Entity;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> DeleteByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            var entitiesToRemove = await _entities.Where(predicate).ToListAsync();

            foreach (var entity in entitiesToRemove)
            {
                _entities.Remove(entity);
            }

            return entitiesToRemove;
        }
    }
}