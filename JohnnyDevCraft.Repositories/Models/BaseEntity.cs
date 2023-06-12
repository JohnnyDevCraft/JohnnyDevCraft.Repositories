using JohnnyDevCraft.Repositories.Abstractions;

namespace JohnnyDevCraft.Repositories.Models;

public class BaseEntity<TKey>: IIdentifiableEntity<TKey>
{
    public TKey Id { get; set; }
}