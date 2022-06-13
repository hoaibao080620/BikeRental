using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shared.Repositories;

public class RepositoryGeneric<T, TV> : IRepositoryGeneric<T> where TV : DbContext where T : class
{
    protected readonly TV Context;

    protected RepositoryGeneric(TV context)
    {
        Context = context;
    }
    
    public async Task<List<T>> All()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task Add(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
    }

    public Task Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task Update(T entity)
    {
        Context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task<List<T>> Find(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<bool> Exists(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().AnyAsync(predicate);
    }

    public async Task SaveChanges()
    {
        await Context.SaveChangesAsync();
    }
}