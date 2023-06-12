using JohnnyDevCraft.Repositories.Abstractions;

namespace JohnnyDevCraft.Repositories.Models;

public class BaseOwnedEntity<Tkey,TTenantKey>: BaseEntity<Tkey>, ITenantOwned<TTenantKey>
{
    public TTenantKey TenantId { get; set; }
    public Tkey Id { get; set; }
}