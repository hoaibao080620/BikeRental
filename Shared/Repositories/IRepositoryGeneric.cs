using System.Linq.Expressions;

namespace Shared.Repositories;

public interface IRepositoryGeneric<T>
{
    Task<List<T>> All();
    Task<T?> GetById(int id);
    Task Add(T entity);
    Task Delete(T entity);
    Task Update(T entity);
    Task<List<T>> Find(Expression<Func<T, bool>> predicate);
}