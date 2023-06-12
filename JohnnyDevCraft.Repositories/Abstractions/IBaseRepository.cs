using System.Linq.Expressions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;

namespace JohnnyDevCraft.Repositories.Abstractions;

public interface IBaseRepository<T, Tkey> where T: class
{
    
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
    Task<T> GetAsync(Tkey key);
    Task<T> AddAsync(T model, CancellationToken cancellationToken = default(CancellationToken));
    Task<T> UpdateAsync(Tkey key, T model, CancellationToken cancellationToken = default(CancellationToken));
    Task<T> PatchAsync(Tkey key, dynamic model, CancellationToken cancellationToken = default(CancellationToken));
    Task DeleteAsync(Tkey key, CancellationToken cancellationToken = default(CancellationToken));
    Task<LoadResult> LoadDataAsync(DataSourceLoadOptionsBase options, CancellationToken cancellationToken = default(CancellationToken));
    IQueryable<T> GetQueryable();
}