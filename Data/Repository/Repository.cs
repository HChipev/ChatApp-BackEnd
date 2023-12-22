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

        public T Add(T entity)
        {
            return _entities.Add(entity).Entity;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public T? FindByCondition(Expression<Func<T, bool>> predicate)
        {
            return _entities.FirstOrDefault(predicate);
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

        public IEnumerable<T> DeleteByCondition(Expression<Func<T, bool>> predicate)
        {
            var entitiesToRemove = _entities.Where(predicate).ToList();

            foreach (var entity in entitiesToRemove)
            {
                _entities.Remove(entity);
            }

            return entitiesToRemove;
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
    }
}