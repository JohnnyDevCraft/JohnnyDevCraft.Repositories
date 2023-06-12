namespace JohnnyDevCraft.Repositories.Abstractions;

public interface ITenantOwned<TKey>
{
    TKey TenantId { get; set; }
}