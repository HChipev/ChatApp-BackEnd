using System.Linq.Expressions;
using Data.Entities.Abstract;

namespace Data.Repository
{
    public interface IRepository<T> where T : IBaseEntity
    {
        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes);
        public T? Find(int id, params Expression<Func<T, object>>[] includes);
        public T Add(T entity);
        public IEnumerable<T> AddRange(IEnumerable<T> entities);
        public T Update(T entity);
        public IEnumerable<T> UpdateRange(IEnumerable<T> entities);
        public T? Delete(int id);
        public IEnumerable<T> DeleteByCondition(Expression<Func<T, bool>> predicate);
        public bool SaveChanges();
        public T? FindByCondition(Expression<Func<T, bool>> predicate);
        public IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> predicate);
    }
}