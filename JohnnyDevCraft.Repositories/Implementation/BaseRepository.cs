using System.Linq.Expressions;
using DevExpress.Data.Linq.Helpers;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using JohnnyDevCraft.Repositories.Abstractions;
using JohnnyDevCraft.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace JohnnyDevCraft.Repositories.Implementation;

public class BaseRepository<T, TKey>: IBaseRepository<T, TKey> where T: class, IIdentifiableEntity<TKey>
{
    protected readonly DbContext _db;
    
    private List<Action<DbSet<T>>> _modifiers;
    protected BaseRepository(DbContext db)
    {
        _db = db;
        _modifiers = new List<Action<DbSet<T>>>();
    }

    protected void ModifySet(Action<DbSet<T>> modifier)
    {
        _modifiers.Add(modifier);
    }

    private static void Copy<TParent, TChild>(TParent parent, TChild child) where TParent : class
    {
        var parentProperties = parent.GetType().GetProperties();
        var childProperties = child.GetType().GetProperties();

        foreach (var parentProperty in parentProperties)
        {
            foreach (var childProperty in childProperties)
            {
                if (parentProperty.Name == childProperty.Name &&
                    parentProperty.PropertyType == childProperty.PropertyType
                    && parentProperty.Name != "TenantId")
                {
                    childProperty.SetValue(child, parentProperty.GetValue(parent));
                    break;
                }
            }
        }
    }

    protected DbSet<T> GetSet()
    {
        var set = _db.Set<T>();
        foreach (var modifier in _modifiers)
        {
            modifier(set);
        }

        return set;
    }
    
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await GetQueryable().ToListAsync(cancellationToken);
    }

    public async Task<T> GetAsync(TKey key)
    {
        return await GetQueryable().SingleOrDefaultAsync(v => Equals(v.Id, key));
    }

    public async Task<T> AddAsync(T? model, CancellationToken cancellationToken = default(CancellationToken))
    {
        var created = GetSet().Add(model);
        await _db.SaveChangesAsync(cancellationToken);
        SetupForSave(model);
        return created.Entity;
    }

    public async Task<T> AddBatchAsync(IEnumerable<T>? models, CancellationToken cancellationToken = default(CancellationToken))
    {
        var created = GetSet().AddRange(models);
        await _db.SaveChangesAsync(cancellationToken);
        SetupForSave(model);
        return created.Entity;
    }

    public virtual async Task<T> UpdateAsync(TKey key, T model, CancellationToken cancellationToken = default(CancellationToken))
    {
        var original = await GetAsync(key);
        ValidateAccess(original);
        Copy(model, original);
        await _db.SaveChangesAsync(cancellationToken);
        return original;
    }

    public async Task<T> PatchAsync(TKey key, dynamic model, CancellationToken cancellationToken = default(CancellationToken))
    {
        var original = await GetAsync(key);
        ValidateAccess(original);
        Copy(model, original);
        await _db.SaveChangesAsync(cancellationToken);
        return original;
    }

    public async Task DeleteAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken))
    {
        var original = await GetAsync(key);
        ValidateAccess(original);
        GetSet().Remove(original);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<LoadResult> LoadDataAsync(DataSourceLoadOptionsBase options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await DataSourceLoader.LoadAsync(GetSet(), options, cancellationToken);
    }

    public virtual IQueryable<T> GetQueryable()
    {
        return GetSet();
    }

    protected async Task<PagedDataResult<T>> GetPagedDataAsync(PagedDataRequest<T> request, Func<T, bool> Selector, CancellationToken cancellationToken = default)
    {
        var filtered = GetQueryable().Where(Selector).AsQueryable();
        var ordered = request.OrderBy == null ? filtered : request.OrderBy(filtered);
        var list = await ordered.Skip(request.Size * request.Page - request.Size).Take(request.Size)
            .ToListAsync(cancellationToken);
        var count = await ordered.CountAsync(cancellationToken);

        return new PagedDataResult<T>()
        {
            Data = list,
            Page = request.Page,
            Size = request.Size,
            Total = count
        };
    }

    protected virtual void SetupForSave(T entity)
    {
       
    }

    protected virtual void ValidateAccess(T entity)
    {
    }
}
