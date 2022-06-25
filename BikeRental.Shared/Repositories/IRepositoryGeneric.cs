using System.Linq.Expressions;

namespace Shared.Repositories;

public interface IRepositoryGeneric<T>
{
    Task<IQueryable<T>> All();
    Task<T?> GetById(int id);
    Task Add(T entity);
    Task Delete(T entity);
    Task Update(T entity);
    Task<IQueryable<T>> Find(Expression<Func<T, bool>> predicate);
    Task<bool> Exists(Expression<Func<T, bool>> predicate);
    Task SaveChanges();
}
