using System.Linq.Expressions;
using Data.Entities.Abstract;

namespace Data.Repository
{
    public interface IRepository<T> where T : IBaseEntity
    {
        IQueryable<T> GetAll();
        T? Find(int id);
        void Add(T entity);
        void Update(T entity);
        T? Remove(int id);
        void Remove(Expression<Func<T, bool>> predicate);
        bool SaveChanges();
        T? FindByCondition(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> predicate);
    }
}