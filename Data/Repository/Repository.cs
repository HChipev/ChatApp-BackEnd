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

        public void Add(T entity)
        {
            _entities.Add(entity);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public T? FindByCondition(Expression<Func<T, bool>> predicate)
        {
            return _entities.FirstOrDefault(predicate);
        }

        public IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> predicate)
        {
            return _entities.Where(predicate).AsQueryable();
        }

        public T? Remove(int id)
        {
            var entity = Find(id);
            if (entity is null)
            {
                return null;
            }

            _entities.Remove(entity);

            return entity;
        }

        public void Update(T obj)
        {
            _entities.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Remove(Expression<Func<T, bool>> predicate)
        {
            var entities = _entities.Where(predicate).ToList();
            if (entities.Count < 1)
            {
                return;
            }

            _entities.RemoveRange(entities);
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
    }
}