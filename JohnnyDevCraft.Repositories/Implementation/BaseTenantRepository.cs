using System.ComponentModel;
using JohnnyDevCraft.Repositories.Abstractions;
using JohnnyDevCraft.Repositories.Constants;
using JohnnyDevCraft.Repositories.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace JohnnyDevCraft.Repositories.Implementation;

public class BaseTenantRepository<T, TKey, TTenantKey>: BaseRepository<T, TKey>
    where T : class, IIdentifiableEntity<TKey>, ITenantOwned<TTenantKey>
{
    private readonly IHttpContextAccessor _context;

    protected BaseTenantRepository(DbContext db, IHttpContextAccessor _context): base(db)
    {
        this._context = _context;
    }
    
    private TTenantKey TenantId
    {
        get
        {
            var stringVal = _context.HttpContext.User.Claims
                .SingleOrDefault(c => c.Type == TenantClaimTypes.TenantId).Value;

            if (string.IsNullOrWhiteSpace(stringVal)) throw new TenantNotSpecifiedException();
            
            var converter = TypeDescriptor.GetConverter(typeof(TTenantKey));
            if (converter.CanConvertFrom(typeof(string)))
            {
                return (TTenantKey)converter.ConvertFromString(stringVal);
            }
            else
            {
                throw new NotSupportedException("Cannot convert string to TTenantKey type.");
            }

        }
    }

    public override IQueryable<T> GetQueryable()
    {
        var included =  base.GetQueryable();
        return included.Where(e => Equals(e.TenantId, TenantId)).AsQueryable();
    }

    protected override void SetupForSave(T entity)
    {
        base.SetupForSave(entity);
        entity.TenantId = TenantId;
    }

    protected override void ValidateAccess(T entity)
    {
        base.ValidateAccess(entity);
        if (!Equals(entity.TenantId, TenantId)) throw new AccessDeniedException();
    }
}