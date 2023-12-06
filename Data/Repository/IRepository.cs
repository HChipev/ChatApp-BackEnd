using System.Linq.Expressions;
using Data.Entities.Abstract;

namespace Data.Repository
{
    public interface IRepository<T> where T : IBaseEntity
    {
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes);
        T? Find(int id, params Expression<Func<T, object>>[] includes);
        void Add(T entity);
        void Update(T entity);
        T? Remove(int id);
        void Remove(Expression<Func<T, bool>> predicate);
        bool SaveChanges();
        T? FindByCondition(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> predicate);
    }
}